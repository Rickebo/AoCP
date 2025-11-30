import { useSettings } from './context/SettingsContext'

export type SummaryConfig = {
  model?: string
  systemPrompt?: string
  userPrompt?: string
  reasoningEffort?: string
  reasoningMaxTokens?: number
}

export type DiscussionConfig = {
  model?: string
  reasoningEffort?: string
  reasoningMaxTokens?: number
}

declare global {
  interface Window {
    setCookie: (url: string, cookie: string) => Promise<void>
    getInput: (year: number, day: number, token: string) => Promise<string>
    getArticle: (year: number, day: number, token: string) => Promise<string>
    getRawDescription: (
      year: number,
      day: number,
      token: string,
      partIndex: number
    ) => Promise<string>
    readFile: (filePath: string) => Promise<string | undefined>
    getProcessedDescription: (
      article: string,
      openRouterToken: string,
      previousArticles?: string[],
      config?: SummaryConfig
    ) => Promise<string | undefined>
    startProcessedDescriptionStream: (
      article: string,
      openRouterToken: string,
      previousArticles?: string[],
      config?: SummaryConfig
    ) => Promise<string | undefined>
    subscribeProcessedDescriptionStream: (
      channel: string,
      listener: (payload: { type: string; content?: string; message?: string }) => void
    ) => () => void
    cancelProcessedDescriptionStream: (channel: string) => Promise<void>
    startDiscussionStream: (
      messages: { role: string; content: string }[],
      openRouterToken: string,
      model?: string,
      reasoning?: { effort?: string; max_tokens?: number }
    ) => Promise<string | undefined>
    subscribeDiscussionStream: (
      channel: string,
      listener: (payload: { type: string; content?: string; message?: string }) => void
    ) => () => void
    cancelDiscussionStream: (channel: string) => Promise<void>
    getDescription: (
      year: number,
      day: number,
      token: string,
      openRouterToken: string
    ) => Promise<{ article: string; processed: string | undefined } | undefined>
  }
}

class AocService {
  private token: string
  private openRouterToken: string
  private summaryConfig: SummaryConfig

  constructor(token: string, openRouterToken: string, summaryConfig: SummaryConfig) {
    this.token = token
    this.openRouterToken = openRouterToken
    this.summaryConfig = summaryConfig
  }

  public async getInput(year: number, day: number): Promise<string> {
    return await window.getInput(year, day, this.token)
  }

  public async getRawDescription(year: number, day: number, partIndex: number): Promise<string> {
    return await window.getRawDescription(year, day, this.token, partIndex)
  }

  public async getProcessedDescription(
    article: string,
    previousArticles: string[] = []
  ): Promise<string | undefined> {
    if (this.openRouterToken == null || this.openRouterToken.trim().length === 0) {
      return undefined
    }

    return await window.getProcessedDescription(
      article,
      this.openRouterToken,
      previousArticles,
      this.summaryConfig
    )
  }

  public async streamProcessedDescription(
    article: string,
    previousArticles: string[] = [],
    handlers: {
      onChunk: (chunk: string) => void
      onDone?: (full?: string) => void
      onError?: (message: string) => void
    }
  ): Promise<{ cancel: () => void } | undefined> {
    if (this.openRouterToken == null || this.openRouterToken.trim().length === 0) {
      return undefined
    }

    const channel = await window.startProcessedDescriptionStream(
      article,
      this.openRouterToken,
      previousArticles,
      this.summaryConfig
    )
    if (channel == null) {
      return undefined
    }

    let fullContent = ''

    const unsubscribe = window.subscribeProcessedDescriptionStream(channel, (payload) => {
      const type = payload?.type
      if (type === 'chunk' && typeof payload.content === 'string') {
        fullContent += payload.content
        handlers.onChunk(payload.content)
      } else if (type === 'done') {
        unsubscribe()
        handlers.onDone?.(fullContent.length > 0 ? fullContent : payload.content)
      } else if (type === 'error') {
        unsubscribe()
        handlers.onError?.(payload.message ?? 'Failed to stream summary')
      }
    })

    const cancel = (): void => {
      unsubscribe()
      void window.cancelProcessedDescriptionStream(channel)
    }

    return { cancel }
  }

  public hasToken(): boolean {
    if (this.token == null || this.token.length < 128) return false

    let token = this.token
    const prefix = 'session='
    if (token.startsWith(prefix)) token = token.slice(prefix.length)

    return token.length == 128
  }
}

export function useAocService(): AocService {
  const settings = useSettings()

  const getSessionToken = (): string => {
    let token = settings.state.aocToken
    if (token == null) return ''

    const prefix = 'session='
    if (token.startsWith(prefix)) token = token.slice(prefix.length)

    return token
  }

  const sessionToken = getSessionToken()
  const openRouterToken = settings.state.openRouterToken ?? ''
  const summaryConfig: SummaryConfig = {
    model: settings.state.summaryModel,
    systemPrompt: settings.state.summarySystemPrompt,
    userPrompt: settings.state.summaryUserPrompt,
    reasoningEffort: settings.state.summaryReasoningEffort,
    reasoningMaxTokens: settings.state.summaryReasoningMaxTokens
  }

  window.setCookie('https://adventofcode.com', sessionToken)

  return new AocService(sessionToken, openRouterToken, summaryConfig)
}
