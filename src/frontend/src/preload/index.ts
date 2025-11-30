import { contextBridge } from 'electron'
import { electronAPI } from '@electron-toolkit/preload'
import { ipcRenderer } from 'electron'

type ProcessedDescriptionStreamPayload = { type: string; content?: string; message?: string }
type SummaryConfig = {
  model?: string
  systemPrompt?: string
  userPrompt?: string
  reasoningEffort?: string
  reasoningMaxTokens?: number
}

// Custom APIs for renderer
const api = {}

// Use `contextBridge` APIs to expose Electron APIs to
// renderer only if context isolation is enabled, otherwise
// just add to the DOM global.
if (process.contextIsolated) {
  console.log('Using isolated context.')
  try {
    contextBridge.exposeInMainWorld('electron', electronAPI)
    contextBridge.exposeInMainWorld('api', api)
    contextBridge.exposeInMainWorld(
      'setCookie',
      (url: string, cookie: string): Promise<void> => ipcRenderer.invoke('set-cookie', url, cookie)
    )
    contextBridge.exposeInMainWorld(
      'getInput',
      (year: number, day: number, token: string): Promise<string> =>
        ipcRenderer.invoke('get-input', year, day, token)
    )
    contextBridge.exposeInMainWorld(
      'getArticle',
      (year: number, day: number, token: string): Promise<string> =>
        ipcRenderer.invoke('get-article', year, day, token)
    )
    contextBridge.exposeInMainWorld(
      'getRawDescription',
      (year: number, day: number, token: string, partIndex: number): Promise<string> =>
        ipcRenderer.invoke('get-raw-description', year, day, token, partIndex)
    )
    contextBridge.exposeInMainWorld(
      'getProcessedDescription',
      (
        article: string,
        openRouterToken: string,
        previousArticles: string[] = [],
        config?: SummaryConfig
      ): Promise<string | undefined> =>
        ipcRenderer.invoke('get-processed-description', article, openRouterToken, previousArticles, config)
    )
    contextBridge.exposeInMainWorld(
      'startProcessedDescriptionStream',
      (
        article: string,
        openRouterToken: string,
        previousArticles: string[] = [],
        config?: SummaryConfig
      ): Promise<string | undefined> =>
        ipcRenderer.invoke(
          'start-processed-description-stream',
          article,
          openRouterToken,
          previousArticles,
          config
        )
    )
    contextBridge.exposeInMainWorld(
      'subscribeProcessedDescriptionStream',
      (
        channel: string,
        listener: (payload: ProcessedDescriptionStreamPayload) => void
      ): (() => void) => {
        const wrapped = (
          _event: Electron.IpcRendererEvent,
          data: ProcessedDescriptionStreamPayload
        ): void => listener(data)
        ipcRenderer.on(channel, wrapped)
        return () => ipcRenderer.removeListener(channel, wrapped)
      }
    )
    contextBridge.exposeInMainWorld(
      'cancelProcessedDescriptionStream',
      (channel: string): Promise<void> =>
        ipcRenderer.invoke('cancel-processed-description-stream', channel)
    )
    contextBridge.exposeInMainWorld(
      'getDescription',
      (
        year: number,
        day: number,
        token: string,
        openRouterToken: string
      ): Promise<{ article: string; processed: string | undefined } | undefined> =>
        ipcRenderer.invoke('get-description', year, day, token, openRouterToken)
    )
    contextBridge.exposeInMainWorld(
      'saveGif',
      (defaultFileName: string, bytes: Uint8Array): Promise<boolean> =>
        ipcRenderer.invoke('save-gif', defaultFileName, bytes)
    )
  } catch (error) {
    console.error(error)
  }
} else {
  console.log('Using non-isolated context.')
  // @ts-ignore (define in dts)
  window.electron = electronAPI
  // @ts-ignore (define in dts)
  window.api = api
  // window.setCookie = (url: string, cookie: string): Promise<void> => ipcRenderer.invoke('set-cookie', url, cookie)
}
