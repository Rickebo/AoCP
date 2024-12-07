import { useEffect, useState } from 'react'

export interface CooldownContext {
  set: (func: CooldownFunction) => void
}

export type CooldownFunction = () => void

export function useCooldown(time: number): CooldownContext {
  const [func, setFunc] = useState<CooldownFunction>()

  useEffect(() => {
    if (func == null) return

    const timeout = setTimeout(() => func(), time)

    return () => {
      window.clearTimeout(timeout)
    }
  }, [func])

  return {
    set: (func: CooldownFunction) => {
      setFunc(() => func)
    }
  }
}
