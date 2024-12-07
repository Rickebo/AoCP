import { FC } from 'react'
import { Button, Form, Stack } from 'react-bootstrap'
import { usePersistentState } from '../StateUtils'
import { v4 as uuid } from 'uuid'

export interface ProblemInputProps {
  className?: string
  problemKey: string
  onSolve: (input: string) => void
}

interface ProblemInputData {
  selected: string | undefined
  inputs: Record<string, string>
}

const ProblemInput: FC<ProblemInputProps> = (props) => {
  const inputCache = usePersistentState<ProblemInputData>(props.problemKey, {
    selected: undefined,
    inputs: {}
  })

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
      <div className="vr" />
      <Button onClick={() => props.onSolve(getInput() ?? '')} disabled={!hasInput}>
        Run
      </Button>
    </Stack>
  )
}

export default ProblemInput
