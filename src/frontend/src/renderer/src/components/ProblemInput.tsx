import { FC, useEffect, useState } from 'react'
import { Button, ButtonGroup, Dropdown, Form, Spinner, Stack } from 'react-bootstrap'
import { usePersistentState } from '../StateUtils'
import { v4 as uuid } from 'uuid'
import { BsCloudDownload } from 'react-icons/bs'
import { useAocService } from '../AocUtils'
import { ProblemDescriptionData } from '@renderer/data/ProblemDescriptionData'

export interface ProblemInputProps {
  year: number
  day: number
  className?: string
  problemKey: string
  onSolve: (input: string) => void
  onRender?: (input: string) => void
}

interface ProblemInputData {
  selected: string | undefined
  inputs: Record<string, string>
}


const ProblemInput: FC<ProblemInputProps> = (props) => {
  const aocService = useAocService()
  const inputCache = usePersistentState<ProblemInputData>(props.problemKey, {
    selected: undefined,
    inputs: {}
  })

  const [loadingInput, setLoadingInput] = useState<boolean>(false)

  const setInput = (newInput: string): void => {
    inputCache.update((current) => {
      const newSelected = current.selected ?? uuid()
      current.selected = newSelected
      current.inputs[newSelected] = newInput
      inputCache.save(current)
    })
  }

  const getInput = (): string | undefined =>
    inputCache.state.selected != null
      ? inputCache.state.inputs[inputCache.state.selected]
      : undefined

  const hasInput = getInput() != null
  const hasToken = aocService.hasToken()

  const downloadInput = async (): Promise<void> => {
    setLoadingInput(true)
    try {
      const response = await aocService.getInput(props.year, props.day)
      setInput(response)
    } finally {
      setLoadingInput(false)
    }
  }

  return (
    <Stack direction="horizontal" className={props.className} gap={3}>
      <Form.Control
        as="textarea"
        placeholder="Your problem input..."
        onChange={(e) => setInput(e.currentTarget.value)}
        value={getInput() ?? ''}
        multiple
        style={{
          fontFamily: 'Source Code Pro, monospace',
          fontWeight: 300
        }}
      />
      <Button onClick={() => downloadInput()} disabled={!hasToken}>
        {!loadingInput ? <BsCloudDownload /> : <Spinner size="sm" />}
      </Button>
      <div className="vr" />
      {props.onRender == null ? (
        <Button onClick={() => props.onSolve(getInput() ?? '')} disabled={!hasInput}>
          Run
        </Button>
      ) : (
        <Dropdown as={ButtonGroup}>
          <Button onClick={() => props.onSolve(getInput() ?? '')} disabled={!hasInput}>
            Run
          </Button>
          <Dropdown.Toggle
            split
            id="problem-input-run-dropdown"
            disabled={!hasInput}
          />
          <Dropdown.Menu>
            <Dropdown.Item
              onClick={() => {
                props.onRender?.(getInput() ?? '')
              }}
            >
              Render
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      )}
    </Stack>
  )
}

export default ProblemInput
