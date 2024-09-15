import { Container, Nav, Navbar, NavDropdown } from 'react-bootstrap'
import { useMetadataService } from '../hooks'
import { FC, useEffect, useState } from 'react'
import { Metadata, ProblemSetMetadata } from '../data/metadata'

export interface NavigationBarProps {
  setProblemSet: (problemSet: ProblemSetMetadata) => void
}

const NavigationBar: FC<NavigationBarProps> = (props) => {
  const metadataService = useMetadataService()
  const [metadata, setMetadata] = useState({} as Metadata)
  const [loaded, setLoaded] = useState<boolean>(false)

  useEffect(() => {
    metadataService.getMetadata().then((data) => {
      setMetadata(data)
      setLoaded(true)
    })
  }, [])

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
                {(collection ?? []).problemSets.map((problemSet) => (
                  <NavDropdown.Item
                    key={`${year}/${problemSet.releaseTime}`}
                    onClick={() => props.setProblemSet(problemSet)}
                  >
                    Day {new Date(problemSet.releaseTime).getDate()} - {problemSet.name}
                  </NavDropdown.Item>
                ))}
              </NavDropdown>
            ))}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}

export default NavigationBar
