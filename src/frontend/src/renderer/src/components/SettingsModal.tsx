import { FC, useEffect, useState } from 'react'
import {
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
                          <CloseButton onClick={() => backend.removeUrl(url)}/>
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
