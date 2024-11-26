import {
  Button,
  CloseButton,
  Container,
  Form,
  InputGroup,
  Nav,
  Navbar,
  NavDropdown,
  Stack
} from 'react-bootstrap'
import { useMetadataService } from '../hooks'
import { FC, useEffect, useState } from 'react'
import { Metadata, ProblemCollectionMetadata, ProblemSetMetadata } from '../data/metadata'
import { useBackend } from '../context/BackendContext'

export interface NavigationBarProps {
  setProblemSet: (year: number, problemSet: ProblemSetMetadata) => void
}

const NavigationBar: FC<NavigationBarProps> = (props) => {
  const backend = useBackend()
  const metadataService = useMetadataService()
  const [metadata, setMetadata] = useState({} as Metadata)
  const [loaded, setLoaded] = useState<boolean>(false)
  const [backendShown, setBackendShown] = useState<boolean>(false)
  const [enteredBackendUrl, setEnteredBackendUrl] = useState<string>('')

  useEffect(() => {
    setMetadata({} as Metadata)
    setLoaded(false)
    metadataService.getMetadata().then((data) => {
      setMetadata(data)
      setLoaded(true)
    })
  }, [backend.url])

  const openGithub = (): void => {
    window.open('https://github.com/Rickebo/AoCP')
  }

  const collections = loaded ? metadata.collections : {}

  return (
    <Navbar>
      <Container fluid>
        <Navbar.Brand>AoCP</Navbar.Brand>
        <Navbar.Toggle />
        <Navbar.Collapse id="navbarScroll">
          <Nav className="gap-3">
            <Nav.Link onClick={openGithub}>GitHub</Nav.Link>
            {Object.entries(collections).map(([year, collection]) => (
              <NavDropdown key={year} title={year}>
                {(collection?.problemSets ?? []).map((problemSet) => (
                  <NavDropdown.Item
                    key={`${year}/${problemSet.releaseTime}`}
                    onClick={() => props.setProblemSet(Number(year), problemSet)}
                  >
                    Day {new Date(problemSet.releaseTime).getDate()} - {problemSet.name}
                  </NavDropdown.Item>
                ))}
              </NavDropdown>
            ))}
            <NavDropdown
              key="backend"
              title="Backend"
              className="ms-auto"
              autoClose="outside"
              show={backendShown}
              onToggle={(open, metadata) => {
                if (metadata.source != 'select') setBackendShown(open)
              }}
            >
              {backend.urls.map((url) => (
                <NavDropdown.Item
                  key={url}
                  onClick={() => {
                    setBackendShown(false)
                    backend.setUrl(url)
                  }}
                >
                  <Stack direction="horizontal">
                    <span
                      style={{
                        fontWeight: backend.url == url ? 700 : undefined
                      }}
                    >
                      {url}
                    </span>
                    <div className="ms-auto" />
                    {url != backend.url ? (
                      <CloseButton
                        onClick={() => backend.removeUrl(url)}
                        style={{
                          fontSize: 12
                        }}
                      />
                    ) : null}
                  </Stack>
                </NavDropdown.Item>
              ))}
              <NavDropdown.Item onClick={() => {}}>
                <Form inline>
                  <InputGroup size="sm">
                    <Form.Control
                      placeholder="new backend..."
                      onChange={(e) => setEnteredBackendUrl(e.currentTarget.value)}
                    />
                    <Button
                      onClick={() => {
                        backend.addUrl(enteredBackendUrl)
                      }}
                    >
                      Add
                    </Button>
                  </InputGroup>
                </Form>
              </NavDropdown.Item>
            </NavDropdown>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}

export default NavigationBar
