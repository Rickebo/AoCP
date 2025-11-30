import React, { createContext, ReactNode, useContext } from 'react'
import { SavableState, usePersistentState } from '../StateUtils'

export interface SettingsData {
  aocToken: string | undefined
  openRouterToken: string | undefined
  retrieveDescription: boolean
  summarizeWithAI: boolean
  summaryModel: string
  summarySystemPrompt: string
  summaryUserPrompt: string
  summaryReasoningEffort?: string
  summaryReasoningMaxTokens?: number
  discussionModel: string
  discussionReasoningEffort?: string
  discussionReasoningMaxTokens?: number
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
    openRouterToken: undefined,
    retrieveDescription: true,
    summarizeWithAI: false,
    summaryModel: 'openai/gpt-5',
    summarySystemPrompt:
      'You summarize Advent of Code problem descriptions into concise HTML while preserving important formatting such as code blocks and distinct input examples. Use any provided earlier parts only as context to interpret references; the output must cover only the target part.',
    summaryUserPrompt:
      'Summarize only the target Advent of Code problem part below into shorter HTML that keeps the essential details needed to solve that part. If earlier parts are provided, treat them purely as context and do not re-summarize them.',
    summaryReasoningEffort: undefined,
    summaryReasoningMaxTokens: undefined,
    discussionModel: 'openai/gpt-5',
    discussionReasoningEffort: undefined,
    discussionReasoningMaxTokens: undefined
  })

  return <SettingsContext.Provider value={settings}>{children}</SettingsContext.Provider>
}
