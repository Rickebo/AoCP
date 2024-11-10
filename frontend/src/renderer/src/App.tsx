import 'bootstrap/dist/css/bootstrap.min.css'
import Layout from './Layout'

document.documentElement.dataset.bsTheme = 'dark'

function App(): JSX.Element {
  return (
    <div className="overflow-hidden h-100">
      <Layout />
    </div>
  )
}

export default App
