import { ProblemId } from '../services/ProblemService'

export interface ProblemUpdate {
  type: 'ongoing' | 'finished'
  id: ProblemId
}

export interface FinishedProblemUpdate extends ProblemUpdate {
  successful: boolean
  solution: string | undefined
  error: string | undefined
}
