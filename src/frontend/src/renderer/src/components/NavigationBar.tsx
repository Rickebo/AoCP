import { Container, Nav, Navbar, NavDropdown, Stack } from 'react-bootstrap'
import { useMetadataService } from '../hooks'
import { FC, useEffect, useState } from 'react'
import { Metadata, ProblemSetMetadata } from '../data/metadata'
import { useBackend } from '../context/BackendContext'
import { BsGear } from 'react-icons/bs'
import SettingsModal from './SettingsModal'
import './NavigationBar.css'

export interface NavigationBarProps {
  setProblemSet: (year: number, problemSet: ProblemSetMetadata) => void
  author: string
  setAuthor: (author: string) => void
}

const NavigationBar: FC<NavigationBarProps> = (props) => {
  const [metadata, setMetadata] = useState({} as Metadata)
  const [loaded, setLoaded] = useState<boolean>(false)

  const backend = useBackend()
  const metadataService = useMetadataService()
  const [authorShown, setAuthorShown] = useState<boolean>(false)
  const [showSettings, setShowSettings] = useState<boolean>(false)

  const authorSet = new Set<string>()
  const collections = loaded ? metadata.collections : {}

  for (const collection of Object.values(collections)) {
    for (const author of Object.keys(collection.problemSets)) {
      authorSet.add(author)
    }
  }

  const authors = [...authorSet]

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

  return (
    <Navbar>
      <Container fluid>
        <SettingsModal show={showSettings} hide={() => setShowSettings(false)} />
        <Navbar.Brand>AoCP</Navbar.Brand>
        <Navbar.Toggle />
        <Navbar.Collapse id="navbarScroll">
          <Nav className="gap-3">
            <Nav.Link onClick={openGithub}>GitHub</Nav.Link>
            {Object.entries(collections).map(([year, collection]) => (
              <NavDropdown key={year} title={year}>
                {(collection?.problemSets[props.author ?? 0] ?? []).map((problemSet) => (
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
              key="author"
              title="Author"
              className="ms-auto"
              autoClose="outside"
              show={authorShown}
              onToggle={(open, metadata) => {
                if (metadata.source != 'select') setAuthorShown(open)
              }}
            >
              {authors.map((currentAuthor) => (
                <NavDropdown.Item
                  key={currentAuthor}
                  onClick={() => {
                    props.setAuthor(currentAuthor)
                    setAuthorShown(false)
                  }}
                >
                  <Stack direction="horizontal">
                    <span
                      style={{
                        fontWeight: props.author == currentAuthor ? 700 : undefined
                      }}
                    >
                      {currentAuthor}
                    </span>
                  </Stack>
                </NavDropdown.Item>
              ))}
            </NavDropdown>
          </Nav>
          <div className="ms-auto" />
          <Nav.Link onClick={() => setShowSettings(true)} className="me-2 hover-btn">
            <BsGear />
          </Nav.Link>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}

export default NavigationBar
