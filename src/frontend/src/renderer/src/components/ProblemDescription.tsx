import { FC, useEffect, useRef, useState } from 'react'
import './ProblemDescription.css'
import { ProblemMetadata } from '../data/metadata'
import { usePersistentState } from '@renderer/StateUtils'
import { ProblemDescriptionData } from '@renderer/data/ProblemDescriptionData'
import { useSettings } from '../context/SettingsContext'
import { useAocService } from '@renderer/AocUtils'
import { BsStars, BsArrowRepeat } from 'react-icons/bs'

export interface ProblemDescriptionProps {
  metadata: ProblemMetadata
  problemKey: string
  year: number
  day: number
  partIndex: number
}

const ProblemDescription: FC<ProblemDescriptionProps> = (props) => {
  const aocService = useAocService()
  const settings = useSettings()
  const [loadingRaw, setLoadingRaw] = useState<boolean>(false)
  const [loadingProcessed, setLoadingProcessed] = useState<boolean>(false)
  const [showSummary, setShowSummary] = useState<boolean>(settings.state.summarizeWithAI)
  const [streamedSummary, setStreamedSummary] = useState<string | undefined>(undefined)
  const streamHandleRef = useRef<{ cancel: () => void }>()

  const cacheKey = `${props.problemKey}-part-${props.partIndex}`

  const descriptionCache = usePersistentState<ProblemDescriptionData>(cacheKey, {
    raw: undefined,
    processed: undefined
  })

  const descriptionData = descriptionCache.state
  const hasRaw = descriptionData?.raw != null
  const hasProcessed = descriptionData?.processed != null

  const summaryEnabled = settings.state.summarizeWithAI && hasRaw
  const summaryHtml = streamedSummary ?? descriptionData?.processed
  const summaryAvailable = summaryHtml != null

  const getCachedDescriptionForPart = (part: number): ProblemDescriptionData | undefined => {
    const key = `${props.problemKey}-part-${part}`
    try {
      const cached = localStorage.getItem(key)
      if (cached == null) return undefined
      return JSON.parse(cached) as ProblemDescriptionData
    } catch (error) {
      console.error('Failed to read cached description', error)
      return undefined
    }
  }

  const cacheRawDescriptionForPart = (part: number, raw: string): void => {
    const key = `${props.problemKey}-part-${part}`
    try {
      const existing = getCachedDescriptionForPart(part) ?? { raw: undefined, processed: undefined }
      const nextState: ProblemDescriptionData = { ...existing, raw }
      localStorage.setItem(key, JSON.stringify(nextState))
    } catch (error) {
      console.error('Failed to cache previous description', error)
    }
  }

  const loadPreviousPartDescriptions = async (): Promise<string[]> => {
    if (props.partIndex <= 0) {
      return []
    }

    const articles: string[] = []
    for (let i = 0; i < props.partIndex; i++) {
      const cachedRaw = getCachedDescriptionForPart(i)?.raw
      if (cachedRaw != null && cachedRaw.length > 0) {
        articles.push(cachedRaw)
        continue
      }

      try {
        const raw = await aocService.getRawDescription(props.year, props.day, i)
        articles.push(raw)
        cacheRawDescriptionForPart(i, raw)
      } catch (error) {
        console.error(`Failed to fetch previous part ${i + 1} description`, error)
      }
    }

    return articles
  }

  const downloadRawDescription = async (force: boolean = false): Promise<string | undefined> => {
    if (!settings.state.retrieveDescription || loadingRaw || (!force && hasRaw)) {
      return undefined
    }

    setLoadingRaw(true)

    try {
      const raw = await aocService.getRawDescription(props.year, props.day, props.partIndex)
      descriptionCache.update((current) => {
        current.raw = raw
        if (force) {
          current.processed = undefined
        }
        descriptionCache.save(current)
      })
      return raw
    } catch (error) {
      console.error('Failed to download raw description', error)
      return undefined
    } finally {
      setLoadingRaw(false)
    }
  }

  const cancelStream = (): void => {
    streamHandleRef.current?.cancel()
    streamHandleRef.current = undefined
  }

  const streamProcessedDescription = async (
    force: boolean = false,
    rawOverride?: string
  ): Promise<void> => {
    if (
      !settings.state.summarizeWithAI ||
      !settings.state.retrieveDescription ||
      loadingProcessed
    ) {
      return
    }

    const current = descriptionCache.state
    const raw = rawOverride ?? current.raw
    if (raw == null || raw.length === 0) {
      return
    }

    if (!force && current.processed != null) {
      return
    }

    cancelStream()
    setLoadingProcessed(true)
    setStreamedSummary('')

    try {
      const previousArticles = await loadPreviousPartDescriptions()

      const handle = await aocService.streamProcessedDescription(raw, previousArticles, {
        onChunk: (chunk) => {
          setStreamedSummary((prev) => (prev ?? '') + chunk)
        },
        onDone: (full) => {
          const finalHtml = full ?? streamedSummary ?? ''
          setStreamedSummary(finalHtml)
          if (finalHtml.length > 0) {
            descriptionCache.update((currentState) => {
              currentState.processed = finalHtml
              descriptionCache.save(currentState)
            })
          }
          setLoadingProcessed(false)
          streamHandleRef.current = undefined
        },
        onError: (message) => {
          console.error('Failed to stream processed description', message)
          setLoadingProcessed(false)
          streamHandleRef.current = undefined
        }
      })

      if (handle != null) {
        streamHandleRef.current = handle
      } else {
        setLoadingProcessed(false)
      }
    } catch (error) {
      console.error('Failed to start streaming processed description', error)
      setLoadingProcessed(false)
      streamHandleRef.current = undefined
    }
  }

  useEffect(() => {
    if (settings.state.retrieveDescription && !hasRaw) {
      void downloadRawDescription()
    }
  }, [hasRaw, settings.state.retrieveDescription])

  useEffect(() => {
    if (
      settings.state.retrieveDescription &&
      settings.state.summarizeWithAI &&
      hasRaw &&
      !hasProcessed &&
      !loadingProcessed
    ) {
      void streamProcessedDescription()
    }
  }, [
    hasRaw,
    hasProcessed,
    loadingProcessed,
    settings.state.retrieveDescription,
    settings.state.summarizeWithAI
  ])

  useEffect(() => {
    if (!settings.state.summarizeWithAI && showSummary) {
      setShowSummary(false)
    }
  }, [settings.state.summarizeWithAI])

  useEffect(() => {
    if (!settings.state.retrieveDescription && showSummary) {
      setShowSummary(false)
    }
  }, [settings.state.retrieveDescription])

  useEffect(() => {
    if (!settings.state.summarizeWithAI || !settings.state.retrieveDescription) {
      cancelStream()
      setStreamedSummary(undefined)
    }
  }, [settings.state.retrieveDescription, settings.state.summarizeWithAI])

  useEffect(() => {
    return (): void => {
      cancelStream()
    }
  }, [])

  let html: string | undefined

  if (settings.state.summarizeWithAI && showSummary && summaryAvailable) {
    html = summaryHtml ?? undefined
  } else if (settings.state.retrieveDescription && descriptionData?.raw != null) {
    html = descriptionData.raw
  } else if (props.metadata.description != null) {
    html = props.metadata.description
  }

  const innerHtml = html != null ? { __html: html } : undefined

  const showSummaryButton = settings.state.summarizeWithAI && hasRaw

  const handleResummarize = async (): Promise<void> => {
    const freshlyDownloaded = await downloadRawDescription(true)
    const rawForSummary = freshlyDownloaded ?? descriptionCache.state.raw

    if (rawForSummary == null || rawForSummary.length === 0) {
      return
    }

    await streamProcessedDescription(true, rawForSummary)
  }

  return (
    <div className="problem-container">
      {showSummaryButton && (
        <button
          type="button"
          className={`btn btn-sm btn-outline-warning problem-button problem-summary-toggle${
            showSummary ? ' active' : ''
          }`}
          onClick={() => {
            if (!summaryEnabled) {
              return
            }
            setShowSummary((current) => !current)
          }}
          disabled={!summaryEnabled}
          title={showSummary ? 'Show full description' : 'Show summarized description'}
        >
          {!loadingProcessed && <BsStars className="problem-button-icon" />}
          {loadingProcessed && (
            <span
              className={`spinner-border spinner-border-sm problem-button-spinner ${
                showSummary ? 'text-dark' : 'text-warning'
              }`}
              role="status"
              aria-hidden="true"
            />
          )}
        </button>
      )}
      {showSummaryButton && showSummary && (
        <button
          type="button"
          className="btn btn-sm btn-outline-primary problem-button problem-refresh-button"
          onClick={() => {
            void handleResummarize()
          }}
          disabled={loadingRaw || loadingProcessed}
          title="Re-summarize description"
        >
          {!loadingProcessed && <BsArrowRepeat className="problem-button-icon" />}
          {loadingProcessed && (
            <span
              className="spinner-border spinner-border-sm text-primary problem-button-spinner"
              role="status"
              aria-hidden="true"
            />
          )}
        </button>
      )}
      <div
        dangerouslySetInnerHTML={innerHtml}
        className={`problem ${showSummary ? 'problem--with-summary' : 'problem--no-summary'}`}
        style={{
          fontFamily: 'Source Code Pro, monospace',
          fontSize: '1em',
          fontWeight: 'normal'
        }}
      />
    </div>
  )
}

export default ProblemDescription
