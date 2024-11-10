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
    <div className={props.className} style={props.style}>
      Solution: {props.solution ?? '?'}
    </div>
  )
}

export default ProblemSolution
