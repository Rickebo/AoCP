import { FC, useEffect, useState } from 'react'
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

  const cacheKey = `${props.problemKey}-part-${props.partIndex}`

  const descriptionCache = usePersistentState<ProblemDescriptionData>(cacheKey, {
    raw: undefined,
    processed: undefined
  })

  const descriptionData = descriptionCache.state
  const hasRaw = descriptionData?.raw != null
  const hasProcessed = descriptionData?.processed != null

  const summaryEnabled = settings.state.summarizeWithAI && hasProcessed

  const downloadRawDescription = async (): Promise<void> => {
    if (!settings.state.retrieveDescription || loadingRaw || hasRaw) {
      return
    }

    setLoadingRaw(true)

    try {
      const raw = await aocService.getRawDescription(props.year, props.day, props.partIndex)
      descriptionCache.update((current) => {
        current.raw = raw
        descriptionCache.save(current)
      })
    } finally {
      setLoadingRaw(false)
    }
  }

  const downloadProcessedDescription = async (): Promise<void> => {
    if (!settings.state.summarizeWithAI || !settings.state.retrieveDescription || loadingProcessed || !hasRaw || hasProcessed) {
      return
    }

    const current = descriptionCache.state
    if (current.raw == null) {
      return
    }

    setLoadingProcessed(true)

    try {
      const processed = await aocService.getProcessedDescription(current.raw)
      if (processed != null) {
        descriptionCache.update((currentState) => {
          currentState.processed = processed
          descriptionCache.save(currentState)
        })
      }
    } finally {
      setLoadingProcessed(false)
    }
  }

  const refreshProcessedDescription = async (): Promise<void> => {
    if (!settings.state.summarizeWithAI || !settings.state.retrieveDescription || loadingProcessed || !hasRaw) {
      return
    }

    const current = descriptionCache.state
    if (current.raw == null) {
      return
    }

    setLoadingProcessed(true)

    try {
      const processed = await aocService.getProcessedDescription(current.raw)
      if (processed != null) {
        descriptionCache.update((currentState) => {
          currentState.processed = processed
          descriptionCache.save(currentState)
        })
      }
    } finally {
      setLoadingProcessed(false)
    }
  }

  useEffect(() => {
    if (settings.state.retrieveDescription && !hasRaw) {
      void downloadRawDescription()
    }
  }, [hasRaw, settings.state.retrieveDescription])

  useEffect(() => {
    if (settings.state.retrieveDescription && settings.state.summarizeWithAI && hasRaw && !hasProcessed) {
      void downloadProcessedDescription()
    }
  }, [hasRaw, hasProcessed, settings.state.retrieveDescription, settings.state.summarizeWithAI])

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

  let html: string | undefined

  if (settings.state.summarizeWithAI && showSummary && hasProcessed) {
    html = descriptionData?.processed ?? undefined
  } else if (settings.state.retrieveDescription && descriptionData?.raw != null) {
    html = descriptionData.raw
  } else if (props.metadata.description != null) {
    html = props.metadata.description
  }

  const innerHtml = html != null ? { __html: html } : undefined

  const showSummaryButton = settings.state.summarizeWithAI && hasRaw

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
            void refreshProcessedDescription()
          }}
          disabled={!hasRaw || loadingProcessed}
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
