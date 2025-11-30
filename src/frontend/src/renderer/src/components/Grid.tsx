import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react'
import useResizeObserver from '@react-hook/resize-observer'
import { Transform } from '../lib/Transform'
import { GifRenderSession, GridGifContext, RenderSettings } from '../services/GifService'
import '../assets/grid.scss'

export interface GridCell {
  char: string | undefined
  glyph: string | undefined
  bg: string | undefined
  fg: string | undefined
}

export type GridData = Record<string, Record<string, string | GridCell>>

export interface GridRef {
  draw: ((grid: GridData) => void) | undefined
  clear: () => void
  setSize: (w: number, h: number) => void
  startRender: (context: GridGifContext, settings: RenderSettings) => void
  stopRender: () => Promise<void>
}

export interface Glyph {
  rows: boolean[][]
}

function glyph(...lines: string[]): Glyph {
  const resultRows: boolean[][] = []
  for (const line of lines) {
    const row: boolean[] = []
    for (let i = 0; i < line.length; i++) {
      row.push(line.charAt(i) == '1')
    }
    resultRows.push(row)
  }

  return {
    rows: resultRows
  }
}

// prettier-ignore
const Glyphs: Record<string, Glyph> = {
  '|': glyph('010', '010', '010'),
  '-': glyph('000', '111', '000'),
  'L': glyph('010', '011', '000'),
  '7': glyph('000', '110', '010'),
  'J': glyph('010', '110', '000'),
  'F': glyph('000', '011', '010'),
  '.': glyph('000', '000', '000'),
  '*': glyph('000', '010', '000'),
  '+': glyph('010', '111', '010'),
  'O': glyph('111', '101', '111'),
  'X': glyph('101', '010', '101'),
  '9': glyph('111', '111', '111'),
  '#': glyph('1'),
  'W': glyph('000', '110', '000'),
  'N': glyph('010', '010', '000'),
  'E': glyph('000', '011', '000'),
  'S': glyph('000', '010', '010'),
  '\u2533': glyph('000', '111', '010'),
  '\u253b': glyph('010', '111', '000'),
  '\u252b': glyph('010', '110', '010'),
  '\u2523': glyph('010', '011', '010')
}

function fillRectangle(
  context: CanvasRenderingContext2D,
  x: number,
  y: number,
  width: number,
  height: number,
  color: string,
  transform: Transform
): void {
  context.fillStyle = color
  const [dx, dy] = transform.transform(x, y)
  const margin = 0.5 // Add some overlap so the border isn't visible in some zooms
  context.fillRect(
    dx - margin,
    dy - margin,
    transform.scale(width) + margin * 2,
    transform.scale(height) + margin * 2
  )
}

function fillGlyph(
  context: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  cell: GridCell,
  transform: Transform
): void {
  if (cell.char != null) {
    if (cell.bg != null) {
      fillRectangle(context, x, y, size, size, cell.bg, transform)
    }

    if (cell.fg != null) {
      const textSize = transform.scale(size)
      context.font = `${textSize}px Pixel Square`
      context.fillStyle = cell.fg
      const [dx, dy] = transform.transform(x, y)
      context.fillText(cell.char, dx, dy + textSize, textSize)
    }
  }

  if (cell.glyph != null) {
    const glyph = Glyphs[cell.glyph]
    const rows = glyph?.rows
    if (rows == null) return

    const gh = size / rows.length
    const gw = size / rows[0].length

    for (let gy = 0; gy < rows.length; gy++) {
      const row = rows[gy]
      for (let gx = 0; gx < row.length; gx++) {
        const color = row[gx] ? cell.fg : cell.bg
        if (!color) continue
        fillRectangle(context, x + gx * gw, y + gy * gh, gw, gh, color, transform)
      }
    }
  }
}

function handlePixel(
  context: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  cell: string | GridCell,
  transform: Transform
): void {
  if (typeof cell === 'string') {
    fillRectangle(context, x, y, size, size, cell, transform)
  } else {
    fillGlyph(context, x, y, size, cell, transform)
  }
}

function scaleFactor(scale: number): number {
  return Math.pow(1.5, scale - 1)
}

const MIN_FILL = 0.9
const MIN_SCALE = 1 + Math.log(MIN_FILL) / Math.log(1.5)

function createTransform(offsetX: number, offsetY: number, scaleCoefficient: number): Transform {
  const factor = scaleFactor(scaleCoefficient)
  return new Transform([offsetX, offsetY], factor)
}

const Grid = forwardRef<GridRef, unknown>((_, ref) => {
  const [divRef, setDivRef] = useState<HTMLDivElement | null>(null)
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const [w, setW] = useState<number>(0)
  const [h, setH] = useState<number>(0)
  const [canvasSize, setCanvasSize] = useState<[number, number]>([0, 0])
  const [data, setData] = useState<GridData>({})
  const [scale, setScale] = useState<number>(1)
  const [offset, setOffset] = useState<[number, number]>([0, 0])
  const [clickPos, setClickPos] = useState<[number, number] | undefined>(undefined)
  const [clickOffset, setClickOffset] = useState<[number, number] | undefined>(undefined)
  const [mousePos, setMousePos] = useState<[number, number] | undefined>(undefined)
  const renderSessionRef = useRef<GifRenderSession | null>(null)
  const hasCenteredInitially = useRef(false)

  const resizeCanvas = (width: number, height: number): void => {
    if (canvasRef.current == null) return

    canvasRef.current.setAttribute('width', width.toString())
    canvasRef.current.setAttribute('height', height.toString())
    canvasRef.current.style.width = `${width}px`
    canvasRef.current.style.height = `${height}px`

    draw(data, true)
  }

  useEffect(() => {
    draw(data, true)
  }, [offset, scale])

  useEffect(() => {
    resizeCanvas(...canvasSize)
  }, [canvasSize])

  useEffect(() => {
    if (divRef == null) return
    setCanvasSize([divRef.clientWidth, divRef.clientHeight])
  }, [divRef])

  useResizeObserver(divRef, () => {
    if (divRef == null) return
    setCanvasSize([divRef.clientWidth, divRef.clientHeight])
  })

  useEffect(() => {
    if (w > 0 && h > 0 && canvasSize[0] > 0 && canvasSize[1] > 0 && !hasCenteredInitially.current) {
      hasCenteredInitially.current = true
      const [canvasWidth, canvasHeight] = canvasSize
      const pixelSize = Math.min(canvasWidth / w, canvasHeight / h)
      const factor = scaleFactor(MIN_SCALE)
      const centeredOffsetX = (canvasWidth / factor - w * pixelSize) / 2
      const centeredOffsetY = (canvasHeight / factor - h * pixelSize) / 2
      if (scale !== MIN_SCALE) setScale(MIN_SCALE)
      setOffset([centeredOffsetX, centeredOffsetY])
    }
  }, [w, h, canvasSize, scale])

  useEffect(() => {
    if (scale !== MIN_SCALE) return
    const [canvasWidth, canvasHeight] = canvasSize
    if (canvasWidth === 0 || canvasHeight === 0 || w === 0 || h === 0) return
    const pixelSize = Math.min(canvasWidth / w, canvasHeight / h)
    const factor = scaleFactor(MIN_SCALE)
    const centeredOffsetX = (canvasWidth / factor - w * pixelSize) / 2
    const centeredOffsetY = (canvasHeight / factor - h * pixelSize) / 2
    setOffset([centeredOffsetX, centeredOffsetY])
  }, [canvasSize, w, h, scale])

  const draw = (data: GridData, clear: boolean = false): void => {
    if (data == null) return
    if (w == 0 || h == 0) return

    const canvas = canvasRef.current
    const context = canvas?.getContext('2d')

    if (canvas == null || context == null) return

    if (clear) {
      context.clearRect(0, 0, canvas.width, canvas.height)
    }

    const canvasWidth = context.canvas?.width ?? canvas?.width ?? 0
    const canvasHeight = context.canvas?.height ?? canvas?.height ?? 0
    const pixelSize = Math.min(canvasWidth / w, canvasHeight / h)
    const newData: GridData = {}
    const [ox, oy] = offset
    const transform = createTransform(ox, oy, scale)

    for (const [yIndex, row] of Object.entries(data)) {
      const y = Number(yIndex)
      if (y >= h) continue

      const yp = (h - 1 - y) * pixelSize
      if (yp > canvasHeight) continue

      const newRow: Record<string, string | GridCell> = (newData[yIndex] = {})

      for (const [xIndex, cell] of Object.entries(row)) {
        const x = Number(xIndex)
        if (x >= w) continue

        const xp = x * pixelSize
        if (xp > canvasWidth) continue

        handlePixel(context, xp, yp, pixelSize, cell, transform)
        newRow[xIndex] = cell
      }
    }

    // Draw border around the grid
    const [x0, y0] = transform.transform(0, 0)
    const [sw, sh] = transform.scale([w * pixelSize, h * pixelSize])
    context.save()
    context.strokeStyle = 'rgba(0, 0, 0, 0.85)'
    context.lineWidth = 2
    context.strokeRect(x0, y0, sw, sh)
    context.strokeStyle = 'rgba(255, 255, 255, 0.9)'
    context.lineWidth = 1
    context.strokeRect(x0, y0, sw, sh)
    context.restore()

    setData((current) => {
      const result: GridData = { ...current }
      for (const [yIndex, row] of Object.entries(newData)) {
        result[yIndex] = { ...result[yIndex], ...row }
      }

      return result
    })

    if (!clear && renderSessionRef.current != null && canvas != null) {
      try {
        renderSessionRef.current.addFrame(canvas)
      } catch {
        // Ignore frame capture failures (e.g. context lost)
      }
    }
  }

  const clear = (): void => {
    if (canvasRef.current == null) return
    canvasRef.current
      ?.getContext('2d')
      ?.clearRect(0, 0, canvasRef.current.width, canvasRef.current.height)
  }

  const zoom = (delta: number): void => {
    if (mousePos == null) return

    setScale((currentScale) => {
      const oldFactor = scaleFactor(currentScale)
      const proposedScale = currentScale + delta

      const minScale = MIN_SCALE
      const maxScale = 10
      const nextScale = Math.max(minScale, Math.min(maxScale, proposedScale))

      if (nextScale === currentScale) return currentScale

      const newFactor = scaleFactor(nextScale)

      const [canvasWidth, canvasHeight] = canvasSize

      if (nextScale === minScale) {
        if (canvasWidth === 0 || canvasHeight === 0 || w === 0 || h === 0) return currentScale
        const pixelSize = Math.min(canvasWidth / w, canvasHeight / h)
        const centeredOffsetX = (canvasWidth / newFactor - w * pixelSize) / 2
        const centeredOffsetY = (canvasHeight / newFactor - h * pixelSize) / 2
        setOffset([centeredOffsetX, centeredOffsetY])
        return nextScale
      }

      const currentWidth = canvasWidth / oldFactor
      const currentHeight = canvasHeight / oldFactor
      const newWidth = canvasWidth / newFactor
      const newHeight = canvasHeight / newFactor

      const mouseX = mousePos[0]
      const mouseY = mousePos[1]

      const dx = (currentWidth - newWidth) * mouseX
      const dy = (currentHeight - newHeight) * mouseY

      setOffset((currentOffset) => {
        const [cx, cy] = currentOffset
        return [cx - dx, cy - dy]
      })

      return nextScale
    })
  }

  const pan = (mx: number, my: number): void => {
    if (clickPos == null || clickOffset == null) return

    const [cx, cy] = clickPos
    const [canvasWidth, canvasHeight] = canvasSize
    const factor = scaleFactor(scale)
    const deltaX = (mx - cx) * (canvasWidth / factor)
    const deltaY = (my - cy) * (canvasHeight / factor)
    const [ox, oy] = clickOffset
    setOffset([ox + deltaX, oy + deltaY])
  }

  const getRelativePosition = (e: React.MouseEvent<HTMLCanvasElement>): [number, number] => {
    const boundingRect = e.currentTarget.getBoundingClientRect()
    return [
      (e.clientX - boundingRect.left) / boundingRect.width,
      (e.clientY - boundingRect.top) / boundingRect.height
    ]
  }

  const startRender = (context: GridGifContext, settings: RenderSettings): void => {
    renderSessionRef.current = new GifRenderSession(context, settings)
  }

  const stopRender = async (): Promise<void> => {
    if (renderSessionRef.current == null) return

    const session = renderSessionRef.current
    renderSessionRef.current = null
    await session.finish()
  }

  useImperativeHandle(ref, (): GridRef => {
    return {
      draw: draw,
      clear: clear,
      setSize: (w: number, h: number): void => {
        setW(w)
        setH(h)
      },
      startRender: (context: GridGifContext, settings: RenderSettings): void => {
        startRender(context, settings)
      },
      stopRender: async (): Promise<void> => {
        await stopRender()
      }
    }
  }, [draw, clear, canvasRef.current])

  return (
    <div
      ref={(r) => setDivRef(r)}
      style={{
        display: 'flex',
        flex: '1 1 auto',
        overflow: 'hidden'
      }}
    >
      <canvas
        ref={canvasRef}
        width={100}
        height={100}
        style={{
          imageRendering: 'crisp-edges'
        }}
        onWheel={(e) => {
          zoom(-e.deltaY / 100)
        }}
        onMouseDown={(e) => {
          setClickPos(getRelativePosition(e))
          setClickOffset(offset)
        }}
        onMouseUp={() => {
          setClickPos(undefined)
          setClickOffset(undefined)
        }}
        onMouseMove={(e) => {
          const [mx, my] = getRelativePosition(e)
          setMousePos([mx, my])

          if (clickPos == null) return
          pan(mx, my)
        }}
      />
    </div>
  )
})

Grid.displayName = 'Grid'

export default Grid
