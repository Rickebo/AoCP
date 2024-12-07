import { MutableRefObject, useCallback, useEffect, useState } from 'react'
import {
  FinishedProblemUpdate,
  GridUpdate,
  ProblemUpdate,
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
  solve: (problem: ProblemMetadata, input: string) => Promise<void>
  solveAll: (input: string) => void
}

export function useConnectionManager(
  year: number,
  author: string,
  set: ProblemSetMetadata,
  grids: MutableRefObject<Record<string, GridRef | null>>
): ProblemFeedbackHandler {
  const problemService = useProblemService()
  const [solutions, setSolutions] = useState<Record<string, string>>({})
  const [log, setLog] = useState<Record<string, string[] | undefined>>({})
  const [, setSocket] = useState<ProblemSocket | undefined>()
  const [gridQueue, setGridQueue] = useState<Record<string, GridData[]>>({})

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

  const handleFinished = (update: FinishedProblemUpdate): void => {
    if (update.solution == null) return

    setSolutions((solutions) => {
      return {
        ...solutions,
        [update.id.problemName]: update.solution!
      }
    })
  }

  const handleStart = (): void => {
    setSolutions({})
    setLog({})
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

      const queue = replacer[problemName] ?? []
      queue.push(update.rows)
      replacer[problemName] = queue

      return replacer
    })

    if (update.clear) grids.current[update.id.problemName]?.clear()
    if (update.width != null && update.height != null)
      grids.current[update.id.problemName]?.setSize(update.width, update.height)
  }

  const handlers: Record<string, (update: ProblemUpdate) => void> = {
    text: (update: ProblemUpdate) => handleLog(update as TextProblemUpdate),
    finished: (update: ProblemUpdate) => handleFinished(update as FinishedProblemUpdate),
    grid: (update: ProblemUpdate) => handleGrid(update as GridUpdate)
  }

  const handleMessage = useCallback((message: MessageEvent) => {
    const data = JSON.parse(message.data) as ProblemUpdate
    handlers[data.type]?.(data)
  }, [])

  const solve = async (problem: ProblemMetadata, input: string): Promise<void> => {
    if (problem.name == null) throw new Error('Cannot solve unnamed problem.')

    const id: ProblemId = {
      year: year,
      author: author,
      setName: set.name,
      problemName: problem.name
    }

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

  return {
    problemService: problemService,
    solution: (name: string): string | undefined => solutions[name],
    log: (name: string): string[] | undefined => log[name],
    solve: solve,
    solveAll: solveAll
  }
}
