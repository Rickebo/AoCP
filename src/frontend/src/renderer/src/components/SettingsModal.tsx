import { FC, useEffect, useState } from 'react'
import {
  Accordion,
  Button,
  Form,
  Modal,
  Stack,
  CloseButton,
  ListGroup,
  ListGroupItem
} from 'react-bootstrap'
import { useSettings } from '../context/SettingsContext'
import { useBackend } from '../context/BackendContext'

export interface SettingsModalProps {
  show: boolean
  hide: () => void
}

const SettingsModal: FC<SettingsModalProps> = (props) => {
  const settings = useSettings()
  const backend = useBackend()

  const [enteredBackendUrl, setEnteredBackendUrl] = useState<string>('')
  const [isEnteredUrlValid, setIsEnteredUrlValid] = useState<boolean>(false)
  const summarizationDisabled = !settings.state.retrieveDescription || !settings.state.summarizeWithAI

  useEffect(() => {
    setIsEnteredUrlValid(isValidUrl(enteredBackendUrl))
  }, [enteredBackendUrl])

  const isValidUrl = (url: string): boolean => {
    try {
      const parsed = new URL(url)
      return parsed.protocol == 'http:' || parsed.protocol == 'https:'
    } catch {
      return false
    }
  }

  return (
    <Modal show={props.show} onHide={() => props.hide()}>
      <Modal.Header closeButton>
        <Modal.Title>Settings</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form>
          <Stack gap={3}>
            <Form.Group>
              <Form.Label>AoC token</Form.Label>
              <Form.Control
                type="text"
                value={settings.state.aocToken ?? ''}
                placeholder="session=...."
                onChange={(e) => {
                  const newToken = e.currentTarget.value
                  settings.update((current) => {
                    current.aocToken = newToken
                    settings.save(current)
                  })
                }}
              />
            </Form.Group>

            <Form.Group>
              <Form.Check
                type="switch"
                label="Retrieve description"
                checked={settings.state.retrieveDescription}
                onChange={(e) => {
                  const enabled = e.currentTarget.checked
                  settings.update((current) => {
                    current.retrieveDescription = enabled
                    if (!enabled) {
                      current.summarizeWithAI = false
                    }
                    settings.save(current)
                  })
                }}
              />
            </Form.Group>

            <Accordion alwaysOpen defaultActiveKey={['backend']}>
              <Accordion.Item eventKey="ai">
                <Accordion.Header>AI Summarization</Accordion.Header>
                <Accordion.Body>
                  <Stack gap={3}>
                    <Form.Group>
                      <Form.Label>OpenRouter API token</Form.Label>
                      <Form.Control
                        type="text"
                        value={settings.state.openRouterToken ?? ''}
                        placeholder="sk-or-..."
                        onChange={(e) => {
                          const newToken = e.currentTarget.value
                          settings.update((current) => {
                            current.openRouterToken = newToken
                            settings.save(current)
                          })
                        }}
                      />
                    </Form.Group>

                    <Form.Group>
                      <Form.Check
                        type="switch"
                        label="Summarize with AI"
                        checked={settings.state.summarizeWithAI}
                        disabled={!settings.state.retrieveDescription}
                        onChange={(e) => {
                          const enabled = e.currentTarget.checked
                          settings.update((current) => {
                            current.summarizeWithAI = enabled
                            settings.save(current)
                          })
                        }}
                      />
                    </Form.Group>

                    <Form.Group>
                      <Form.Label>Summarization model</Form.Label>
                      <Form.Control
                        type="text"
                        placeholder="openai/gpt-5"
                        disabled={summarizationDisabled}
                        value={settings.state.summaryModel}
                        onChange={(e) => {
                          const model = e.currentTarget.value
                          settings.update((current) => {
                            current.summaryModel = model
                            settings.save(current)
                          })
                        }}
                      />
                    </Form.Group>

                    <Form.Group>
                      <Form.Label>Summarization system prompt</Form.Label>
                      <Form.Control
                        as="textarea"
                        rows={3}
                        disabled={summarizationDisabled}
                        value={settings.state.summarySystemPrompt}
                        onChange={(e) => {
                          const prompt = e.currentTarget.value
                          settings.update((current) => {
                            current.summarySystemPrompt = prompt
                            settings.save(current)
                          })
                        }}
                      />
                    </Form.Group>

                    <Form.Group>
                      <Form.Label>Summarization user prompt prefix</Form.Label>
                      <Form.Control
                        as="textarea"
                        rows={3}
                        disabled={summarizationDisabled}
                        value={settings.state.summaryUserPrompt}
                        onChange={(e) => {
                          const prompt = e.currentTarget.value
                          settings.update((current) => {
                            current.summaryUserPrompt = prompt
                            settings.save(current)
                          })
                        }}
                      />
                      <Form.Text>
                        The selected article HTML is appended after this text when requesting a summary.
                      </Form.Text>
                    </Form.Group>

                    <Form.Group>
                      <Form.Label>Reasoning effort (OpenRouter)</Form.Label>
                      <Form.Select
                        disabled={summarizationDisabled}
                        value={settings.state.summaryReasoningEffort ?? ''}
                        onChange={(e) => {
                          const effort = e.currentTarget.value
                          settings.update((current) => {
                            current.summaryReasoningEffort = effort.length > 0 ? effort : undefined
                            settings.save(current)
                          })
                        }}
                      >
                        <option value="">Use model default</option>
                        <option value="none">None</option>
                        <option value="minimal">Minimal</option>
                        <option value="low">Low</option>
                        <option value="medium">Medium</option>
                        <option value="high">High</option>
                      </Form.Select>
                      <Form.Text>
                        Maps to OpenRouter <code>reasoning.effort</code>. Ignored if a reasoning token
                        budget is set below.
                      </Form.Text>
                    </Form.Group>

                    <Form.Group>
                      <Form.Label>Reasoning max tokens</Form.Label>
                      <Form.Control
                        type="number"
                        min={0}
                        disabled={summarizationDisabled}
                        value={settings.state.summaryReasoningMaxTokens ?? ''}
                        placeholder="Leave blank for none"
                        onChange={(e) => {
                          const raw = e.currentTarget.value
                          const parsed = Number(raw)
                          settings.update((current) => {
                            current.summaryReasoningMaxTokens =
                              raw.length === 0 || !Number.isFinite(parsed) || parsed <= 0
                                ? undefined
                                : Math.floor(parsed)
                            settings.save(current)
                          })
                        }}
                      />
                      <Form.Text>
                        Uses OpenRouter <code>reasoning.max_tokens</code>. If set to a positive number,
                        it takes precedence over effort.
                      </Form.Text>
                    </Form.Group>
                  </Stack>
                </Accordion.Body>
              </Accordion.Item>

              <Accordion.Item eventKey="backend">
                <Accordion.Header>Backend</Accordion.Header>
                <Accordion.Body>
                  <Form.Group>
                    <Form.Label>Backend</Form.Label>
                    <ListGroup>
                      {backend.urls.map((url) => (
                        <ListGroupItem key={url} className="pe-2">
                          <Stack direction="horizontal">
                            <span
                              style={{
                                fontWeight: backend.url == url ? 700 : undefined
                              }}
                            >
                              {url}
                            </span>
                            <div className="ms-auto" />
                            {url == backend.url ? null : (
                              <>
                                <Button size="sm" className="me-2" onClick={() => backend.setUrl(url)}>
                                  Activate
                                </Button>
                                <CloseButton onClick={() => backend.removeUrl(url)} />
                              </>
                            )}
                          </Stack>
                        </ListGroupItem>
                      ))}
                      <ListGroupItem className="ps-2 py-1">
                        <Stack direction="horizontal" gap={3}>
                          <Form.Control
                            style={{ border: '0' }}
                            placeholder="new backend..."
                            value={enteredBackendUrl}
                            onChange={(e) => setEnteredBackendUrl(e.currentTarget.value)}
                          />
                          <Button
                            onClick={() => {
                              backend.addUrl(enteredBackendUrl)
                              backend.setUrl(enteredBackendUrl)
                              setEnteredBackendUrl('')
                            }}
                            size="sm"
                            disabled={!isEnteredUrlValid}
                          >
                            Add
                          </Button>
                        </Stack>
                      </ListGroupItem>
                    </ListGroup>
                  </Form.Group>
                </Accordion.Body>
              </Accordion.Item>
            </Accordion>
          </Stack>
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={props.hide}>Close</Button>
      </Modal.Footer>
    </Modal>
  )
}

export default SettingsModal
