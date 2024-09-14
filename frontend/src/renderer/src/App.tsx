import 'bootstrap/dist/css/bootstrap.min.css'
import { Container } from 'react-bootstrap'
import Layout from './Layout'

document.documentElement.dataset.bsTheme = 'dark'

function App(): JSX.Element {
  return (
    <Container className="position-absolute inset">
      <Layout />
    </Container>
  )
}

export default App
