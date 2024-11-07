import { FC } from 'react'
import { ProblemMetadata } from '../data/metadata'

export interface ProblemSolutionProps {
  problem: ProblemMetadata
  solution: string | undefined
}

const ProblemSolution: FC<ProblemSolutionProps> = (props) => {
  return (
    <div>
      Solution: {props.solution ?? '?'}
    </div>
  )
}

export default ProblemSolution
