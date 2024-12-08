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
