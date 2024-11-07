import { ProblemSetMetadata } from '@renderer/data/metadata'
import { FC, useState } from 'react'
import Problem from './Problem'
import ProblemInput from './ProblemInput'
import { ProblemMetadata } from '../data/metadata'
import { ProblemId } from '../services/ProblemService'
import { useProblemService } from '../hooks'
import ProblemSolution from './ProblemSolution'
import { Accordion } from 'react-bootstrap'

export interface ProblemSetProps {
  year: number
  set: ProblemSetMetadata
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
  const problemService = useProblemService()
  const [solutions, setSolutions] = useState<Record<string, string>>({})

  const solve = async (problem: ProblemMetadata, input: string): Promise<void> => {
    if (problem.name == null) throw new Error('Cannot solve unnamed problem.')

    const id: ProblemId = {
      year: props.year,
      setName: props.set.name,
      problemName: problem.name
    }

    const solution = await problemService.solve(id, input)

    setSolutions((current) => {
      const newSolutions = { ...current }
      newSolutions[problem.name!] = solution.successful ? solution.solution : solution.error
      return newSolutions
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
