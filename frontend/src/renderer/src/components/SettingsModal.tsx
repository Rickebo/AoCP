import { FC } from 'react'
import { Button, Form, Modal } from 'react-bootstrap'
import { useSettings } from '../context/SettingsContext'

export interface SettingsModalProps {
  show: boolean
  hide: () => void
}

const SettingsModal: FC<SettingsModalProps> = (props) => {
  const settings = useSettings()

  return (
    <Modal show={props.show}>
      <Modal.Header closeButton>
        <Modal.Title>Settings</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form>
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
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button onClick={props.hide}>Close</Button>
      </Modal.Footer>
    </Modal>
  )
}

export default SettingsModal
