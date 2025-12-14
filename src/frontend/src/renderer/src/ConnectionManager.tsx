import { MutableRefObject, useCallback, useEffect, useState } from 'react'
import {
  FinishedProblemUpdate,
  GridUpdate,
  ProblemUpdate,
  TableUpdate,
  TextProblemUpdate
} from './data/ProblemUpdate'
import ProblemSocket from './services/ProblemSocket'
import { useProblemService } from './hooks'
import ProblemService, { ProblemId } from './services/ProblemService'
import { ProblemMetadata, ProblemSetMetadata } from './data/metadata'
import { GridData, GridRef } from './components/Grid'

export interface ProblemFeedbackHandler {
  problemService: ProblemService
  solution: (problemName: string) => string | undefined
  log: (problemName: string) => string[] | undefined
  table: (problemName: string) => TableData | undefined
  solve: (problem: ProblemMetadata, input: string) => Promise<void>
  solveAll: (input: string) => void
  getSolveData: (problemName: string | undefined) => SolveData | undefined
  elapsed: (problemName: string) => string | undefined
  solving: string[]
}

export interface TableData {
  columns: TableUpdate['columns']
  rows: string[][]
}

export interface SolveData {
  start: Date
  end: Date | undefined
  elapsedNs: number | undefined
}

export function useConnectionManager(
  year: number,
  source: string,
  author: string,
  set: ProblemSetMetadata,
  grids: MutableRefObject<Record<string, GridRef | null>>
): ProblemFeedbackHandler {
  const problemService = useProblemService()
  const [solutions, setSolutions] = useState<Record<string, string>>({})
  const [log, setLog] = useState<Record<string, string[] | undefined>>({})
  const [tables, setTables] = useState<Record<string, TableData | undefined>>({})
  const [, setSocket] = useState<ProblemSocket | undefined>()
  const [gridQueue, setGridQueue] = useState<Record<string, GridData[]>>({})
  const [solveTimes, setSolveTimes] = useState<Record<string, SolveData>>({})
  const [solving, setSolving] = useState<string[]>([])

  useEffect(() => {
    if (Object.entries(gridQueue).length < 1) return
    let update = false
    const clone = { ...gridQueue }

    for (const [name, queue] of Object.entries(clone)) {
      const ref = grids.current[name]
      if (ref?.draw == null || queue.length === 0) continue

      for (const item of queue) ref.draw(item)

      clone[name] = []
      update = true
    }

    if (update) {
      setGridQueue(clone)
    }
  }, [gridQueue, grids.current])

  const getElapsed = (problemName: string): string | undefined => {
    const data = solveTimes[problemName]

    if (data == null) return undefined

    const start = data.start
    const end = data.end ?? new Date()

    const elapsed =
      data.elapsedNs != null ? data.elapsedNs / 1000000 : end.getTime() - start.getTime()

    const micros = (elapsed * 1000) % 1000
    const nanos = (elapsed * 1000000) % 1000
    const millis = elapsed % 1000
    const seconds = Math.floor(elapsed / 1000) % 60
    const minutes = Math.floor(elapsed / (60 * 1000))

    const unitMap: [number, string][] = [
      [minutes, 'm'],
      [seconds, 's'],
      [millis, 'ms'],
      [micros, 'μs'],
      [nanos, 'ns']
    ]

    let usedUnits = 0
    let result = ''
    for (const [value, unit] of unitMap) {
      if (usedUnits >= 2) break

      if (value < 1) {
        if (usedUnits > 0) break
        continue
      }

      const valueStr =
        usedUnits == 0
          ? Math.floor(value).toString()
          : Math.floor(value)
              .toString()
              .padStart(unit != 's' ? 3 : 0, '0')
      usedUnits++
      result += valueStr + unit + ' '
    }

    return result.trimEnd()
  }

  const handleFinished = (update: FinishedProblemUpdate): void => {
    if (update.solution == null) return

    setSolveTimes((current) => {
      const clone = { ...current }
      const data = clone[update.id.problemName]
      if (data == null) return clone

      data.end = new Date()
      data.elapsedNs = update.elapsedNanoseconds
      return clone
    })

    setSolutions((solutions) => {
      return {
        ...solutions,
        [update.id.problemName]: update.solution!
      }
    })

    setSolving((current) => [...current.filter((x) => x != update.id.problemName)])

    const gridRef = grids.current[update.id.problemName]
    if (gridRef?.stopRender != null) {
      void gridRef.stopRender()
    }
  }

  const handleStart = (): void => {
    setSolutions({})
    setLog({})
    setTables({})
  }

  const handleLog = (update: TextProblemUpdate): void => {
    setLog((current) => {
      const newLog = current[update.id.problemName] ?? []

      if (update.text != null) {
        if (newLog.length == 0) {
          newLog.push(update.text)
        } else {
          newLog[newLog.length - 1] += update.text
        }
      }

      if (update.lines != null) {
        newLog.push(...update.lines)
      }

      return {
        ...current,
        [update.id.problemName]: newLog
      }
    })
  }

  const handleGrid = (update: GridUpdate): void => {
    setGridQueue((current) => {
      const problemName = update.id.problemName

      const replacer = {
        ...current
      }

      const queue = update.clear ? [] : (replacer[problemName] ?? [])
      queue.push(update.rows)
      replacer[problemName] = queue

      return replacer
    })

    if (update.clear) grids.current[update.id.problemName]?.clear()
    if (update.width != null && update.height != null)
      grids.current[update.id.problemName]?.setSize(update.width, update.height)
  }

  const handleTable = (update: TableUpdate): void => {
    const problemName = update.id.problemName
    setTables((current) => {
      const existing = current[problemName]
      const nextRows =
        (update.reset ?? true) ? update.rows : [...(existing?.rows ?? []), ...update.rows]

      return {
        ...current,
        [problemName]: {
          columns: update.columns,
          rows: nextRows
        }
      }
    })
  }

  const handlers: Record<string, (update: ProblemUpdate) => void> = {
    text: (update: ProblemUpdate) => handleLog(update as TextProblemUpdate),
    finished: (update: ProblemUpdate) => handleFinished(update as FinishedProblemUpdate),
    grid: (update: ProblemUpdate) => handleGrid(update as GridUpdate),
    table: (update: ProblemUpdate) => handleTable(update as TableUpdate)
  }

  const handleMessage = useCallback((message: MessageEvent) => {
    const data = JSON.parse(message.data) as ProblemUpdate
    handlers[data.type]?.(data)
  }, [])

  const solve = async (problem: ProblemMetadata, input: string): Promise<void> => {
    if (problem.name == null) throw new Error('Cannot solve unnamed problem.')
    setSolving((current) => [...current, problem.name!])

    const id: ProblemId = {
      year: year,
      source: source,
      author: author,
      setName: set.name,
      problemName: problem.name
    }

    setSolveTimes((times) => {
      const clone = { ...times }
      clone[problem.name!] = {
        start: new Date(),
        end: undefined,
        elapsedNs: undefined
      }

      return clone
    })

    problemService.solve(id, input).then((socket) => {
      setSocket(socket)
      socket.addHandler(handleMessage)
    })
  }

  const solveAll = (input: string): void => {
    handleStart()
    for (const problem of set.problems) {
      if (problem.name == null) continue
      solve(problem, input)
    }
  }

  const getSolveData = (problemName: string | undefined): SolveData | undefined =>
    problemName != null ? solveTimes[problemName] : undefined

  return {
    problemService: problemService,
    solution: (name: string): string | undefined => solutions[name],
    log: (name: string): string[] | undefined => log[name],
    table: (name: string): TableData | undefined => tables[name],
    solve: solve,
    solveAll: solveAll,
    getSolveData: getSolveData,
    solving: solving,
    elapsed: getElapsed
  }
}
