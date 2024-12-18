import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react'
import useResizeObserver from '@react-hook/resize-observer'
import { Transform } from '../lib/Transform'
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
  '#': glyph('111', '111', '111')
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
  context.fillRect(dx, dy, transform.scale(width), transform.scale(height))
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

      const yp = y * pixelSize
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

    setData((current) => {
      const result: GridData = { ...current }
      for (const [yIndex, row] of Object.entries(newData)) {
        result[yIndex] = { ...result[yIndex], ...row }
      }

      return result
    })
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
      // const [cx, cy] = mousePos
      const oldScale = scaleFactor(scale)
      const newScale = scale + delta

      const [canvasWidth, canvasHeight] = canvasSize

      const currentWidth = canvasWidth / oldScale
      const newWidth = canvasWidth / newScale

      const currentHeight = canvasHeight / oldScale
      const newHeight = canvasHeight / newScale

      const dx = (currentWidth - newWidth) * mousePos[0]
      const dy = (currentHeight - newHeight) * mousePos[1]

      if (newScale < 1 || newScale > 10) return currentScale

      setOffset((currentOffset) => {
        const [cx, cy] = currentOffset
        return [cx - dx, cy - dy]
      })

      return newScale
    })
  }

  const pan = (mx: number, my: number): void => {
    if (clickPos == null || clickOffset == null) return

    const [cx, cy] = clickPos
    const [w, h] = canvasSize
    const wFactor = w / h
    const dx = (mx - cx) * wFactor
    const dy = my - cy

    const factor = Math.min(...canvasSize) / scaleFactor(scale)

    const [ox, oy] = clickOffset
    const nx = ox + dx * factor
    const ny = oy + dy * factor

    setOffset([nx, ny])
  }

  const getRelativePosition = (e: React.MouseEvent<HTMLCanvasElement>): [number, number] => {
    const boundingRect = e.currentTarget.getBoundingClientRect()
    return [
      (e.clientX - boundingRect.left) / boundingRect.width,
      (e.clientY - boundingRect.top) / boundingRect.height
    ]
  }

  useImperativeHandle(ref, (): GridRef => {
    return {
      draw: draw,
      clear: clear,
      setSize: (w: number, h: number): void => {
        setW(w)
        setH(h)
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
