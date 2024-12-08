class ProblemSocket {
  webSocket: WebSocket
  onMessage: ((message: MessageEvent) => void) | undefined = undefined

  constructor(socket: WebSocket) {
    this.webSocket = socket
    this.webSocket.onmessage = this.handleMessage.bind(this)
  }

  private handleMessage(message: MessageEvent): void {
    this.onMessage?.(message)
  }

  public addHandler(handler: (message: MessageEvent) => void): void {
    this.webSocket.addEventListener('message', handler)
  }
}


export default ProblemSocket
