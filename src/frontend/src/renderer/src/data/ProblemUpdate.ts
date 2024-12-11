import { ProblemId } from '../services/ProblemService'

export interface ProblemUpdate {
  type: 'ongoing' | 'finished' | 'text'
  id: ProblemId
}

export interface FinishedProblemUpdate extends ProblemUpdate {
  successful: boolean
  solution: string | undefined
  elapsedNanoseconds: number
  error: string | undefined
}

export interface OngoingProblemUpdate extends ProblemUpdate {}

export interface StartProblemUpdate extends OngoingProblemUpdate {}

export interface TextProblemUpdate extends OngoingProblemUpdate {
  text: string
  lines: string[]
}

export interface GridUpdate extends OngoingProblemUpdate {
  clear: boolean
  width: number
  height: number
  rows: Record<string, Record<string, string>>
}
