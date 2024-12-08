import { useSettings } from './context/SettingsContext'

declare global {
  interface Window {
    setCookie: (url: string, cookie: string) => Promise<void>
    getInput: (year: number, day: number, token: string) => Promise<string>
  }
}

class AocService {
  private token: string

  constructor(token: string) {
    this.token = token
  }

  public async getInput(year: number, day: number): Promise<string> {
    return await window.getInput(year, day, this.token)
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
  window.setCookie('https://adventofcode.com', sessionToken)

  return new AocService(sessionToken)
}
