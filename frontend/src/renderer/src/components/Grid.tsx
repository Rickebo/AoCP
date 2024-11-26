import { forwardRef, useImperativeHandle, useRef, useState } from 'react'
import useResizeObserver from '@react-hook/resize-observer'

export type GridData = Record<string, Record<string, string>>

export interface GridProps {
}

export interface GridRef {
  draw: ((grid: GridData) => void) | undefined
  clear: () => void
  setSize: (w: number, h: number) => void
}

const Grid = forwardRef<GridRef, GridProps>((props, ref) => {
  const divRef = useRef<HTMLDivElement>(null)
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const [w, setW] = useState<number>(0)
  const [h, setH] = useState<number>(0)
  const [data, setData] = useState<GridData>({})

  useResizeObserver(divRef, (entry: ResizeObserverEntry) => {
    if (canvasRef.current == null || divRef.current == null) return

    const min = Math.min(entry.contentRect.width, entry.contentRect.height)

    canvasRef.current.setAttribute('width', min.toString())
    canvasRef.current.setAttribute('height', min.toString())
    canvasRef.current.style.width = `${min}px`
    canvasRef.current.style.height = `${min}px`

    draw(data)
  })

  const draw = (data: GridData): void => {
    if (data == null) return
    if (w == 0 || h == 0) return

    const canvas = canvasRef.current
    const context = canvas?.getContext('2d')

    if (canvas == null || context == null) return

    const canvasWidth = context.canvas?.width ?? canvas?.width ?? 0
    const canvasHeight = context.canvas?.height ?? canvas?.height ?? 0
    const pixelSize = Math.floor(Math.min(canvasWidth / w, canvasHeight / w))
    const newData: GridData = {}

    for (const [yIndex, row] of Object.entries(data)) {
      const y = Number(yIndex)
      if (y >= h) continue

      const yp = y * pixelSize
      if (yp > canvasHeight) continue

      const newRow: Record<string, string> = newData[yIndex] = {}

      for (const [xIndex, color] of Object.entries(row)) {
        const x = Number(xIndex)
        if (x >= w) continue

        const xp = x * pixelSize
        if (xp > canvasWidth) continue

        context.fillStyle = color
        context.fillRect(xp, yp, pixelSize, pixelSize)
        newRow[xIndex] = color
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
      ref={divRef}
      style={{
        display: 'flex',
        flex: '1 1 auto',
        overflow: 'hidden'
      }}
    >
      <canvas ref={canvasRef} />
    </div>
  )
})

Grid.displayName = 'Grid'

export default Grid
