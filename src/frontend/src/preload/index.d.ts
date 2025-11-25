import { ElectronAPI } from '@electron-toolkit/preload'

declare global {
  interface Window {
    electron: ElectronAPI
    api: unknown
    saveGif: (defaultFileName: string, bytes: Uint8Array) => Promise<boolean>
  }
}
