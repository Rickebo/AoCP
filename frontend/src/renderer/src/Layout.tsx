import { FC } from 'react'
import NavigationBar from './components/NavigationBar'
import { Container } from 'react-bootstrap'

const Layout: FC = () => {
  return (
    <Container className="m-1">
      <NavigationBar />
    </Container>

  )
}

export default Layout
