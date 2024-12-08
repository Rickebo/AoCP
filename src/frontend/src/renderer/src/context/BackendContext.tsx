import React, { createContext, ReactNode, useContext } from 'react'
import { usePersistentState } from '../StateUtils'

interface BackendContextType {
  url: string
  urls: string[]
  setUrl: (url: string) => void
  addUrl: (url: string) => void
  removeUrl: (url: string) => void
}

interface BackendState {
  url: number | undefined
  urls: string[]
}

const BackendContext = createContext<BackendContextType | undefined>(undefined)

export const useBackend = (): BackendContextType => {
  const ctx = useContext(BackendContext)
  if (ctx == null) throw new Error('useBackend called outside BackendProvider.')
  return ctx
}

export const BackendProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const defaultUrl = 'https://localhost:8080'
  const state = usePersistentState<BackendState>('backend_options', {
    url: 0,
    urls: [defaultUrl]
  })

  const value: BackendContextType = {
    url: state.state.urls[state.state.url ?? 0],
    urls: state.state.urls,
    setUrl: (url: string) => {
      state.update((current) => {
        const index = current.urls.indexOf(url)
        if (index >= 0) current.url = index
        state.save(current)
      })
    },
    addUrl: (url: string) => {
      state.update((current) => {
        current.urls.push(url)
        if (current.url == null) current.url = current.urls.length - 1
        state.save(current)
      })
    },
    removeUrl: (url: string) => {
      state.update((current) => {
        const index = current.urls.indexOf(url)
        if (index < 0) return

        if (current.url == index) current.url = undefined
        else if (current.url != null && current.url > index) current.url--

        current.urls.splice(index, 1)
        state.save(current)
      })
    }
  }

  return <BackendContext.Provider value={value}>{children}</BackendContext.Provider>
}
