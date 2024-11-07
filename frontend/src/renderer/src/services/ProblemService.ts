export interface ProblemOutput {
  successful: boolean
  solution: string
  error: string
}

export interface ProblemId {
  year: number
  setName: string
  problemName: string
}

export default class ProblemService {
  private api: string

  constructor(api: string) {
    this.api = api
  }

  public async solve(problemId: ProblemId, input: string): Promise<ProblemOutput> {
    const yearPart = problemId.year.toString()
    const setPart = encodeURIComponent(problemId.setName)
    const problemPart = encodeURIComponent(problemId.problemName)

    const response = await fetch(`${this.api}/problem/solve/${yearPart}/${setPart}/${problemPart}`, {
      method: 'POST',
      body: input
    })

    if (!response.ok) {
      throw new Error(`Failed to solve problem: ${response.statusText}`)
    }

    const data = await response.json()
    return data as ProblemOutput
  }
}
