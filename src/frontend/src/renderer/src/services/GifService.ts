import WebMWriter from 'webm-writer'

declare global {
  interface Window {
    saveGif: (defaultFileName: string, bytes: Uint8Array) => Promise<boolean>
  }
}

export interface GridGifContext {
  year: number
  setName: string
  problemName: string
}

export interface RenderSettings {
  width: number
  height: number
  speedFactor: number
}

const sanitizeFilePart = (value: string): string =>
  value.replace(/[^a-z0-9]+/gi, '_').replace(/^_+|_+$/g, '')

type WebMWriterInstance = {
  addFrame: (frame: HTMLCanvasElement) => void
  complete: () => Promise<Blob>
}

export class GifRenderSession {
  private readonly writer: WebMWriterInstance
  private readonly settings: RenderSettings
  private frameIndex: number = 0
  private readonly offscreen: HTMLCanvasElement
  private readonly offscreenCtx: CanvasRenderingContext2D | null
  private hasFrame: boolean = false
  private readonly context: GridGifContext

  constructor(context: GridGifContext, settings: RenderSettings, fps: number = 20) {
    this.context = context
    this.settings = {
      width: settings.width,
      height: settings.height,
      speedFactor: Math.max(1, Math.floor(settings.speedFactor) || 1)
    }

    this.offscreen = document.createElement('canvas')
    this.offscreen.width = this.settings.width
    this.offscreen.height = this.settings.height
    this.offscreenCtx = this.offscreen.getContext('2d')

    this.writer = new WebMWriter({
      quality: 0.95,
      frameRate: fps,
      frameWidth: this.settings.width,
      frameHeight: this.settings.height
    }) as WebMWriterInstance
  }

  public addFrame(canvas: HTMLCanvasElement): void {
    if (this.offscreenCtx == null) return

    this.frameIndex++
    if (this.frameIndex % this.settings.speedFactor !== 0) return

    this.offscreenCtx.clearRect(0, 0, this.offscreen.width, this.offscreen.height)
    this.offscreenCtx.drawImage(canvas, 0, 0, this.offscreen.width, this.offscreen.height)

    this.writer.addFrame(this.offscreen)
    this.hasFrame = true
  }

  public async finish(): Promise<void> {
    if (!this.hasFrame) return

    const blob: Blob = await this.writer.complete()
    const arrayBuffer = await blob.arrayBuffer()
    const bytes = new Uint8Array(arrayBuffer)

    const fileName = [
      'aoc',
      this.context.year.toString(),
      sanitizeFilePart(this.context.setName),
      sanitizeFilePart(this.context.problemName)
    ]
      .filter((part) => part.length > 0)
      .join('_')
      .concat('.webm')

    await window.saveGif(fileName, bytes)
  }
}
