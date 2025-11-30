import { FC, useEffect, useMemo, useRef, useState } from 'react'
import { Alert, Button, Form, Spinner, Stack } from 'react-bootstrap'
import { useAocService } from '@renderer/AocUtils'
import { useSettings } from '../context/SettingsContext'
import { ProblemMetadata } from '../data/metadata'
import { DiscussionService, ChatMessage } from '../services/DiscussionService'
import { ProblemDescriptionData } from '@renderer/data/ProblemDescriptionData'
import { marked } from 'marked'
import { usePersistentState } from '@renderer/StateUtils'

export interface ProblemDiscussionProps {
  year: number
  day: number
  partIndex: number
  problemKey: string
  problem: ProblemMetadata
  solutionFilePath?: string | undefined
}

const ProblemDiscussion: FC<ProblemDiscussionProps> = (props) => {
  const aocService = useAocService()
  const settings = useSettings()
  const [systemMessage, setSystemMessage] = useState<ChatMessage | undefined>(undefined)
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [input, setInput] = useState<string>('')
  const [loadingContext, setLoadingContext] = useState<boolean>(false)
  const [contextError, setContextError] = useState<string | undefined>(undefined)
  const [streaming, setStreaming] = useState<boolean>(false)
  const [reloadSolution, setReloadSolution] = useState<boolean>(false)
  const streamHandleRef = useRef<{ cancel: () => void }>()

  type PersistedDiscussion = { system?: ChatMessage; messages: ChatMessage[] }
  const storageKey = `${props.problemKey}-discussion-part-${props.partIndex}`
  const persisted = usePersistentState<PersistedDiscussion>(storageKey, {
    system: undefined,
    messages: []
  })

  const discussionService = useMemo(
    () =>
      new DiscussionService(settings.state.openRouterToken ?? '', settings.state.summaryModel ?? 'openai/gpt-5'),
    [settings.state.openRouterToken, settings.state.summaryModel]
  )

  const cacheKey = `${props.problemKey}-part-${props.partIndex}`

  const loadCachedDescription = (): string | undefined => {
    try {
      const cached = localStorage.getItem(cacheKey)
      if (cached == null) return undefined
      const parsed = JSON.parse(cached) as ProblemDescriptionData
      return parsed?.raw
    } catch (error) {
      console.error('Failed to read cached description', error)
      return undefined
    }
  }

  const cacheRawDescription = (raw: string): void => {
    try {
      const parsed = (JSON.parse(localStorage.getItem(cacheKey) ?? '{}') as ProblemDescriptionData) ?? {
        raw: undefined,
        processed: undefined
      }
      parsed.raw = raw
      localStorage.setItem(cacheKey, JSON.stringify(parsed))
    } catch (error) {
      console.error('Failed to cache description', error)
    }
  }

  const loadContext = async (): Promise<void> => {
    setLoadingContext(true)
    setContextError(undefined)
    setMessages([])
    setSystemMessage(undefined)
    streamHandleRef.current?.cancel()

    try {
      let description = loadCachedDescription() ?? props.problem.description

      if (settings.state.retrieveDescription) {
        try {
          description = await aocService.getRawDescription(props.year, props.day, props.partIndex)
          cacheRawDescription(description)
        } catch (error) {
          console.warn('Failed to download problem description, falling back to metadata', error)
        }
      }

      let solutionContent: string | undefined
      if (props.solutionFilePath != null) {
        try {
          solutionContent = await window.readFile(props.solutionFilePath)
        } catch (error) {
          console.error('Failed to read solution file', error)
          setContextError('Could not read the solution file for this problem.')
        }
      }

      const header = `You are an AI assistant helping with Advent of Code ${props.year}, day ${props.day}, part ${
        props.partIndex + 1
      }.`

      const descriptionSection =
        description != null && description.length > 0
          ? `Problem description (raw HTML):\n${description}`
          : 'Problem description is unavailable.'

      const solutionSection =
        props.solutionFilePath != null
          ? `User solution file: ${props.solutionFilePath}\n${
              solutionContent ?? '[Solution file not available or unreadable.]'
            }`
          : 'No solution file path was provided.'

      const guidance =
        'Use the context above to answer questions, spot bugs, and suggest improvements. Prefer concise answers that reference the relevant code lines.'

      const system = {
        role: 'system' as const,
        content: [header, descriptionSection, solutionSection, guidance].join('\n\n')
      }

      setSystemMessage(system)
      const initialMessages: ChatMessage[] = [
        {
          role: 'assistant',
          content:
            'Context loaded. Ask a question about the problem statement or your implementation, and I will respond using both.'
        }
      ]
      setMessages(initialMessages)
      persisted.update((state) => {
        state.system = system
        state.messages = initialMessages
      })
    } finally {
      setLoadingContext(false)
    }
  }

  useEffect(() => {
    const hasPersisted = persisted.state.system != null && persisted.state.messages.length > 0
    if (hasPersisted) {
      setSystemMessage(persisted.state.system)
      setMessages(persisted.state.messages)
      setLoadingContext(false)
    } else {
      void loadContext()
    }
    return () => {
      streamHandleRef.current?.cancel()
    }
  }, [props.problemKey, props.partIndex, props.solutionFilePath])

  const handleSend = async (): Promise<void> => {
    const text = input.trim()
    if (text.length === 0 || systemMessage == null || streaming) return

    // Build AI message list separate from what we display, so injected code stays hidden.
    const aiMessages: ChatMessage[] = [systemMessage, ...messages]

    if (reloadSolution && props.solutionFilePath != null) {
      try {
        const latestContent = await window.readFile(props.solutionFilePath)
        if (latestContent != null) {
          aiMessages.push({
            role: 'user',
            content: `Latest solution file (${props.solutionFilePath}):\n${latestContent}`
          })
        }
      } catch (error) {
        console.error('Failed to reload solution file for discussion', error)
        setContextError('Could not reload the solution file; message sent without updated code.')
      }
    }

    const userMessage: ChatMessage = { role: 'user', content: text }
    const baseMessages = [...messages, userMessage]
    const assistantIndex = baseMessages.length
    const placeholder: ChatMessage = { role: 'assistant', content: '' }

    const nextMessages = [...baseMessages, placeholder]
    setMessages(nextMessages)
    persisted.update((state) => {
      state.system = systemMessage
      state.messages = nextMessages
    })
    setInput('')
    setStreaming(true)

    const handle = await discussionService.streamReply([...aiMessages, userMessage], {
      onChunk: (chunk) => {
        setMessages((current) => {
          const clone = [...current]
          const existing = clone[assistantIndex] ?? { role: 'assistant', content: '' }
          clone[assistantIndex] = { ...existing, content: (existing.content ?? '') + chunk }
          persisted.update((state) => {
            state.system = systemMessage
            state.messages = clone
          })
          return clone
        })
      },
      onDone: (full) => {
        setMessages((current) => {
          const clone = [...current]
          const existing = clone[assistantIndex] ?? { role: 'assistant', content: '' }
          clone[assistantIndex] = { ...existing, content: full ?? existing.content }
          persisted.update((state) => {
            state.system = systemMessage
            state.messages = clone
          })
          return clone
        })
        setStreaming(false)
        streamHandleRef.current = undefined
      },
      onError: (message) => {
        console.error('Discussion stream failed', message)
        setContextError(message ?? 'Discussion stream failed.')
        setStreaming(false)
        streamHandleRef.current = undefined
      }
    })

    if (handle != null) {
      streamHandleRef.current = handle
    } else {
      setStreaming(false)
    }

    // Auto-uncheck after send
    setReloadSolution(false)
  }

  const resetConversation = (): void => {
    if (systemMessage == null) return
    streamHandleRef.current?.cancel()
    setMessages([
      {
        role: 'assistant',
        content:
          'Context loaded. Ask a question about the problem statement or your implementation, and I will respond using both.'
      }
    ])
    persisted.update((state) => {
      state.system = systemMessage
      state.messages = [
        {
          role: 'assistant',
          content:
            'Context loaded. Ask a question about the problem statement or your implementation, and I will respond using both.'
        }
      ]
    })
    setInput('')
    setStreaming(false)
    setContextError(undefined)
  }

  const missingToken = discussionService.hasToken() === false

  return (
    <div className="h-100 d-flex flex-column gap-3 p-3">
      {loadingContext && (
        <div className="d-flex align-items-center gap-2">
          <Spinner animation="border" size="sm" />
          <span>Preparing context from the problem description and your solution file…</span>
        </div>
      )}

      {contextError != null && <Alert variant="warning">{contextError}</Alert>}
      {missingToken && (
        <Alert variant="danger">
          Add an OpenRouter token in Settings to chat with the AI about this problem.
        </Alert>
      )}

      <div
        className="flex-grow-1 overflow-auto border rounded p-3 d-flex flex-column"
        style={{ background: '#0f141b', gap: '0.5rem' }}
      >
        {messages.map((msg, idx) => {
          const isUser = msg.role === 'user'
          const html = marked.parse(msg.content ?? '')

          return (
            <div
              key={idx}
              className="d-flex"
              style={{
                justifyContent: isUser ? 'flex-end' : 'flex-start'
              }}
            >
              <div
                className="p-2 rounded"
                style={{
                  maxWidth: '75%',
                  background: isUser ? '#1f2a3a' : '#0c1118',
                  border: '1px solid #1f2a3a',
                  textAlign: isUser ? 'right' : 'left'
                }}
              >
                <div
                  className={`fw-semibold text-secondary small text-uppercase mb-1 ${
                    isUser ? 'text-end' : 'text-start'
                  }`}
                >
                  {msg.role}
                </div>
                <div
                  className="discussion-message"
                  dangerouslySetInnerHTML={{ __html: html }}
                />
              </div>
            </div>
          )
        })}
      </div>

      <Form
        onSubmit={(e): void => {
          e.preventDefault()
          void handleSend()
        }}
      >
        <Stack direction="horizontal" gap={2}>
          <Form.Control
            as="textarea"
            rows={2}
            value={input}
            onChange={(e) => setInput(e.currentTarget.value)}
            placeholder="Ask the AI about this problem or your implementation…"
            disabled={loadingContext || missingToken}
          />
          <div className="d-flex flex-column gap-2">
            <Form.Check
              type="checkbox"
              id={`${storageKey}-reload-solution`}
              label="Attach latest solution file"
              checked={reloadSolution}
              onChange={(e) => setReloadSolution(e.currentTarget.checked)}
              disabled={loadingContext || missingToken || props.solutionFilePath == null}
              title={
                props.solutionFilePath == null
                  ? 'No solution file path available for this problem'
                  : 'Read the solution file again before sending'
              }
              className="small"
            />
            <Stack gap={2} direction="horizontal">
              <Button
                variant="primary"
                type="submit"
                disabled={loadingContext || missingToken || streaming || systemMessage == null}
              >
                {streaming ? 'Streaming…' : 'Send'}
              </Button>
              <Button variant="outline-secondary" type="button" onClick={resetConversation}>
                Reset
              </Button>
            </Stack>
          </div>
        </Stack>
      </Form>
    </div>
  )
}

export default ProblemDiscussion
