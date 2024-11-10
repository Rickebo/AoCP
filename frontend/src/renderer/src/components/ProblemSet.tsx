import { ProblemSetMetadata } from '@renderer/data/metadata'
import React, { CSSProperties, FC, useCallback, useState } from 'react'
import Problem from './Problem'
import ProblemInput from './ProblemInput'
import { ProblemMetadata } from '../data/metadata'
import { ProblemId } from '../services/ProblemService'
import { useProblemService } from '../hooks'
import ProblemSolution from './ProblemSolution'
import { Accordion, Tab, Tabs } from 'react-bootstrap'
import classNames from 'classnames'
import ProblemSocket from '../services/ProblemSocket'
import {
  FinishedProblemUpdate,
  ProblemUpdate,
  TextProblemUpdate
} from '../data/ProblemUpdate'
import ProblemLog from './ProblemLog'
import { BsArrowRight } from 'react-icons/bs'

export interface ProblemSetProps {
  year: number
  set: ProblemSetMetadata
}

export interface ProblemTitleProps {
  className?: string | undefined
  style?: CSSProperties | undefined
  textClassName?: string | undefined
  textStyle?: CSSProperties | undefined
  parts: string[]
  delimiter: React.ReactNode
}

const ProblemTitle: FC<ProblemTitleProps> = (props) => {
  return (
    <div className={classNames(props.className, 'd-inline')} style={props.style}>
      {props.parts.map((part, i) => (
        <>
          <h3
            key={i}
            className={classNames('d-inline', props.textClassName)}
            style={props.textStyle}
          >
            {part}
          </h3>
          {i == props.parts.length - 1 ? null : (
            <h3
              className={classNames('d-inline', props.textClassName)}
              style={props.textStyle}
            >
              &nbsp;
              {props.delimiter}
              &nbsp;
            </h3>
          )}
        </>
      ))}
    </div>
  )
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
  const problemService = useProblemService()
  const [, setSocket] = useState<ProblemSocket | undefined>()

  const [solutions, setSolutions] = useState<Record<string, string>>({})
  const [log, setLog] = useState<Record<string, string[] | undefined>>({})

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

  const handlers: Record<string, (update: ProblemUpdate) => void> = {
    text: (update) => handleLog(update as TextProblemUpdate),
    finished: (update: ProblemUpdate) => handleFinished(update as FinishedProblemUpdate)
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

  const handleMessage = useCallback((message: MessageEvent) => {
    const data = JSON.parse(message.data) as ProblemUpdate
    handlers[data.type]?.(data)
  }, [])

  const solve = async (problem: ProblemMetadata, input: string): Promise<void> => {
    if (problem.name == null) throw new Error('Cannot solve unnamed problem.')

    const id: ProblemId = {
      year: props.year,
      setName: props.set.name,
      problemName: problem.name
    }

    problemService.solve(id, input)
      .then((socket) => {
        setSocket(socket)
        socket.addHandler(handleMessage)
      })
  }

  const solveAll = (input: string): void => {
    handleStart()
    for (const problem of props.set.problems) {
      if (problem.name == null) continue
      solve(problem, input)
    }
  }

  return (
    <div className="gap-3">
      <ProblemTitle
        delimiter={<BsArrowRight />}
        parts={[
          props.year.toString(),
          props.set.name
        ]}
        textStyle={{
          fontFamily: 'Source Code Pro, monospace',
          fontWeight: 800
        }}
      />

      <ProblemInput className="mb-3" onSolve={solveAll} />

      <Tabs>
        {props.set.problems.map((problem, i) => (
          <Tab key={problem.name} eventKey={problem.name ?? i.toString()}
               title={problem.name}>
            <div className="mt-2 overflow-auto">
              <Problem key={problem.name} metadata={problem} />

              {problem.name == null || log[problem.name] == null ? null : (
                <Accordion className="mt-3">
                  <Accordion.Item eventKey="log" title="Log">
                    <ProblemLog content={log[problem.name]!} />
                  </Accordion.Item>
                </Accordion>
              )}

              {problem.name == null || solutions[problem.name] == null ? null : (
                <ProblemSolution
                  key={problem.name}
                  problem={problem}
                  solution={solutions[problem.name!]}
                  className="mt-3"
                />
              )}
            </div>
          </Tab>
        ))}
      </Tabs>
    </div>
  )
}

export default ProblemSet
