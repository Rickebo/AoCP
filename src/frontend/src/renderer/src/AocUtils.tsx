import { useSettings } from './context/SettingsContext'

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
    getProcessedDescription: (
      article: string,
      openRouterToken: string
    ) => Promise<string | undefined>
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

  constructor(token: string, openRouterToken: string) {
    this.token = token
    this.openRouterToken = openRouterToken
  }

  public async getInput(year: number, day: number): Promise<string> {
    return await window.getInput(year, day, this.token)
  }

  public async getRawDescription(year: number, day: number, partIndex: number): Promise<string> {
    return await window.getRawDescription(year, day, this.token, partIndex)
  }

  public async getProcessedDescription(article: string): Promise<string | undefined> {
    if (this.openRouterToken == null || this.openRouterToken.trim().length === 0) {
      return undefined
    }

    return await window.getProcessedDescription(article, this.openRouterToken)
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

  window.setCookie('https://adventofcode.com', sessionToken)

  return new AocService(sessionToken, openRouterToken)
}
