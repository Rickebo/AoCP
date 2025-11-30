export type ChatMessage = { role: 'system' | 'user' | 'assistant'; content: string }

export interface DiscussionHandlers {
  onChunk: (chunk: string) => void
  onDone?: (full?: string) => void
  onError?: (message: string) => void
}

export class DiscussionService {
  private token: string
  private model: string

  constructor(openRouterToken: string, model: string) {
    this.token = openRouterToken
    this.model = model
  }

  public hasToken(): boolean {
    return typeof this.token === 'string' && this.token.trim().length > 0
  }

  public async streamReply(
    messages: ChatMessage[],
    handlers: DiscussionHandlers
  ): Promise<{ cancel: () => void } | undefined> {
    if (!this.hasToken()) {
      handlers.onError?.('Missing OpenRouter token. Add one in Settings to chat with the AI.')
      return undefined
    }

    const channel = await window.startDiscussionStream(messages, this.token, this.model)
    if (channel == null) {
      handlers.onError?.('Failed to start discussion stream.')
      return undefined
    }

    let full = ''

    const unsubscribe = window.subscribeDiscussionStream(channel, (payload) => {
      const type = payload?.type
      if (type === 'chunk' && typeof payload.content === 'string') {
        full += payload.content
        handlers.onChunk(payload.content)
      } else if (type === 'done') {
        unsubscribe()
        handlers.onDone?.(full.length > 0 ? full : payload.content)
      } else if (type === 'error') {
        unsubscribe()
        handlers.onError?.(payload.message ?? 'Discussion stream failed.')
      }
    })

    const cancel = (): void => {
      unsubscribe()
      void window.cancelDiscussionStream(channel)
    }

    return { cancel }
  }
}
