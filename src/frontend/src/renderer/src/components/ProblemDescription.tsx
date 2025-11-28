import { FC, useEffect, useState } from 'react'
import './ProblemDescription.css'
import { ProblemMetadata } from '../data/metadata'
import { usePersistentState } from '@renderer/StateUtils'
import { ProblemDescriptionData } from '@renderer/data/ProblemDescriptionData'
import { useSettings } from '../context/SettingsContext'
import { useAocService } from '@renderer/AocUtils'
import { BsStars } from 'react-icons/bs'

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
          className={`btn btn-sm btn-outline-warning problem-summary-toggle${
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
          <div>
            <BsStars />
            {loadingProcessed && (
              <span
                className="spinner-border spinner-border-sm text-warning ms-2"
                role="status"
                aria-hidden="true"
              />
            )}
          </div>
        </button>
      )}
      <div
        dangerouslySetInnerHTML={innerHtml}
        className="problem"
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
