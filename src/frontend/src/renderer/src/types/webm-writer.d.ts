declare module 'webm-writer' {
  interface WebMWriterOptions {
    quality?: number
    fileWriter?: unknown
    frameRate?: number
    frameWidth?: number
    frameHeight?: number
  }

  export default class WebMWriter {
    constructor(options?: WebMWriterOptions)
    addFrame(frame: HTMLCanvasElement): void
    complete(): Promise<Blob>
  }
}
