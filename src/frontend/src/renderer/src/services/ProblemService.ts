import ProblemSocket from './ProblemSocket'

export interface ProblemOutput {
  successful: boolean
  solution: string
  error: string
}

export interface ProblemId {
  year: number
  author: string
  setName: string
  problemName: string
}

export default class ProblemService {
  private api: string

  constructor(api: string) {
    this.api = api
  }

  public async solve(problemId: ProblemId, input: string): Promise<ProblemSocket> {
    const yearPart = problemId.year.toString()
    const authorPart = encodeURIComponent(problemId.author)
    const setPart = encodeURIComponent(problemId.setName)
    const problemPart = encodeURIComponent(problemId.problemName)

    const ws = new WebSocket(
      `${this.api}/problem/solve/${yearPart}/${authorPart}/${setPart}/${problemPart}`
    )

    ws.addEventListener('open', () => {
      ws.send(input)
    })

    return new ProblemSocket(ws)
  }
}
