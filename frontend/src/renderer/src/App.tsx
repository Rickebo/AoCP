import 'bootstrap/dist/css/bootstrap.min.css'
import Layout from './Layout'
import { BackendProvider } from './context/BackendContext'

document.documentElement.dataset.bsTheme = 'dark'

function App(): JSX.Element {
  return (
    <div className="overflow-hidden h-100">
      <BackendProvider>
        <Layout />
      </BackendProvider>
    </div>
  )
}

export default App
