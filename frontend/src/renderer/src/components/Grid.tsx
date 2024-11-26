import { forwardRef, useEffect, useImperativeHandle, useRef } from 'react'

export type GridData = Record<string, Record<string, string>>

export interface GridProps {
  displayWidth: number
  displayHeight: number
  width: number
  height: number
}

export interface GridRef {
  draw: ((grid: GridData) => void) | undefined
  clear: () => void
}

const Grid = forwardRef<GridRef, GridProps>((props, ref) => {
  const canvasRef = useRef<HTMLCanvasElement | undefined>(undefined)

  const draw = (data: GridData): void => {
    const canvas = canvasRef.current
    if (canvas == null) return

    const context = canvas.getContext('2d')

    const canvasWidth = context.canvas?.width ?? canvas?.width ?? 0
    const canvasHeight = context.canvas?.height ?? canvas?.height ?? 0
    const pixelSize = Math.min(canvasWidth / props.width, canvasHeight / props.height)

    for (const [yIndex, row] of Object.entries(data)) {
      const y = Number(yIndex)
      if (y >= props.height) continue

      const yp = y * pixelSize
      if (yp > canvasHeight) continue

      for (const [xIndex, color] of Object.entries(row)) {
        const x = Number(xIndex)
        if (x >= props.width) continue

        const xp = x * pixelSize
        if (xp > canvasWidth) continue

        context.fillStyle = color
        context.fillRect(xp, yp, pixelSize, pixelSize)
      }
    }
  }

  const clear = (): void => {
    if (canvasRef.current == null) return
    canvasRef.current
      .getContext('2d')
      .clearRect(0, 0, canvasRef.current.width, canvasRef.current.height)
  }

  useImperativeHandle(ref, (): GridRef => {
    return {
      draw: draw,
      clear: clear
    }
  }, [draw, clear, canvasRef.current])

  return <canvas ref={canvasRef} width={props.displayWidth} height={props.displayHeight} />
})

Grid.displayName = 'Grid'

export default Grid
