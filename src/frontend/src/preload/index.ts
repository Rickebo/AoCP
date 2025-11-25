import { contextBridge } from 'electron'
import { electronAPI } from '@electron-toolkit/preload'
import { ipcRenderer } from 'electron'

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
      (article: string, openRouterToken: string): Promise<string | undefined> =>
        ipcRenderer.invoke('get-processed-description', article, openRouterToken)
    )
    contextBridge.exposeInMainWorld(
      'getDescription',
      (year: number, day: number, token: string, openRouterToken: string): Promise<{article: string, processed: string | undefined} | undefined> => 
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
