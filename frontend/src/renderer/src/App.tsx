import 'bootstrap/dist/css/bootstrap.min.css'
import Layout from './Layout'
import { BackendProvider } from './context/BackendContext'
import { SettingsProvider } from './context/SettingsContext'

document.documentElement.dataset.bsTheme = 'dark'

function App(): JSX.Element {
  return (
    <div className="overflow-hidden h-100">
      <SettingsProvider>
        <BackendProvider>
          <Layout />
        </BackendProvider>
      </SettingsProvider>
    </div>
  )
}

export default App
