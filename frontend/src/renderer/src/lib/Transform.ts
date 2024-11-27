type ScaleResult<T> = T extends number ? number : [number, number]

export class Transform {
  offset: [number, number]
  scaleFactor: [number, number]

  constructor(offset: [number, number], scale: [number, number] | number) {
    this.offset = offset
    this.scaleFactor = typeof scale === 'number' ? [scale, scale] : scale
  }

  private transformIndex(value: number, index: number): number {
    return (value + this.offset[index]) * this.scaleFactor[index]
  }

  public transform(x: number, y: number): [number, number] {
    return [this.transformIndex(x, 0), this.transformIndex(y, 1)]
  }

  public scale<T extends number | [number, number]>(value: T): ScaleResult<T> {
    if (typeof value === 'number') {
      return value * this.scaleFactor[0]
    } else {
      return [value[0] * this.scaleFactor[0], value[1] * this.scaleFactor[1]]
    }
  }
}
