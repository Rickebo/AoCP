import { CSSProperties, FC } from 'react'
import { ProblemMetadata } from '../data/metadata'

export interface ProblemSolutionProps {
  className?: string | undefined
  style?: CSSProperties | undefined
  problem: ProblemMetadata
  solution: string | undefined
}

const ProblemSolution: FC<ProblemSolutionProps> = (props) => {
  return (
    <span>
      Solution: {props.solution ?? '?'}
    </span>
  )
}

export default ProblemSolution
