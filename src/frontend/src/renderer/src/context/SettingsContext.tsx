import React, { createContext, ReactNode, useContext } from 'react'
import { SavableState, usePersistentState } from '../StateUtils'

export interface SettingsData {
  aocToken: string | undefined
  openRouterToken: string | undefined
}

const SettingsContext = createContext<SavableState<SettingsData> | undefined>(undefined)

export const useSettings = (): SavableState<SettingsData> => {
  const ctx = useContext(SettingsContext)
  if (ctx == null) throw new Error('useBackend called outside BackendProvider.')
  return ctx
}

export const SettingsProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const settings = usePersistentState<SettingsData>('settings', {
    aocToken: undefined,
    openRouterToken: undefined
  })

  return <SettingsContext.Provider value={settings}>{children}</SettingsContext.Provider>
}
