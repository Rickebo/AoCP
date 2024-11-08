import { ProblemSetMetadata } from '@renderer/data/metadata'
import { FC, useCallback, useState } from 'react'
import Problem from './Problem'
import ProblemInput from './ProblemInput'
import { ProblemMetadata } from '../data/metadata'
import { ProblemId } from '../services/ProblemService'
import { useProblemService } from '../hooks'
import ProblemSolution from './ProblemSolution'
import { Accordion } from 'react-bootstrap'
import ProblemSocket from '../services/ProblemSocket'
import {
  FinishedProblemUpdate,
  ProblemUpdate,
  TextProblemUpdate
} from '../data/ProblemUpdate'
import ProblemLog from './ProblemLog'

export interface ProblemSetProps {
  year: number
  set: ProblemSetMetadata
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
  const problemService = useProblemService()
  const [solutions, setSolutions] = useState<Record<string, string>>({})
  const [socket, setSocket] = useState<ProblemSocket | undefined>()
  const [log, setLog] = useState<Record<string, string[] | undefined>>({})

  const handleMessage = useCallback((message: MessageEvent) => {
    const data = JSON.parse(message.data) as ProblemUpdate
    if (data.type == 'text') {
      setLog((current) => {
        const textData = data as TextProblemUpdate
        const newLog = current[data.id.problemName] ?? []

        if (textData.text != null) {
          if (newLog.length == 0) {
            newLog.push(textData.text)
          } else {
            newLog[newLog.length - 1] += textData.text
          }
        }

        if (textData.lines != null) {
          newLog.push(...textData.lines)
        }

        return {
          ...current,
          [data.id.problemName]: newLog
        }
      })
    } else if (data.type == 'finished') {
      const finishedData = data as FinishedProblemUpdate
      if (finishedData.solution == null) return

      setSolutions((solutions) => {
        return {
          ...solutions,
          [data.id.problemName]: finishedData.solution!
        }
      })
    }
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
    for (const problem of props.set.problems) {
      if (problem.name == null) continue
      solve(problem, input)
    }
  }

  return (
    <div className="gap-3">
      <h1>{props.set.name}</h1>
      <ProblemInput className="mb-3 px-2" onSolve={solveAll} />
      <Accordion>
        {props.set.problems.map((problem, i) => (
          <Accordion.Item key={problem.name} eventKey={problem.name ?? i.toString()}>
            <Accordion.Header>
              {problem.name}
            </Accordion.Header>
            <Accordion.Body>
              <Problem key={problem.name} metadata={problem} />

              {problem.name == null || log[problem.name] == null ? null : (
                <ProblemLog content={log[problem.name]!} />
              )}

              {problem.name == null || solutions[problem.name] == null ? null : (
                <ProblemSolution
                  key={problem.name}
                  problem={problem}
                  solution={solutions[problem.name!]}
                />
              )}
            </Accordion.Body>
          </Accordion.Item>
        ))}
      </Accordion>
    </div>
  )
}

export default ProblemSet
