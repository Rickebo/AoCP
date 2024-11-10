import { FC, useState } from 'react'
import { Button, Form, Stack } from 'react-bootstrap'

export interface ProblemInputProps {
  className?: string
  onSolve: (input: string) => void
}

const ProblemInput: FC<ProblemInputProps> = (props) => {
  const [input, setInput] = useState<string>('')

  return (
    <Stack direction="horizontal" className={props.className} gap={3}>
      <Form.Control
        as="textarea"
        placeholder="Your problem input..."
        onChange={(e) => setInput(e.currentTarget.value)}
        multiple
        style={{
          fontFamily: 'Source Code Pro, monospace',
          fontWeight: 300

        }}
      />
      <div className="vr" />
      <Button onClick={() => props.onSolve(input)}>
        Run
      </Button>
    </Stack>
  )
}

export default ProblemInput
