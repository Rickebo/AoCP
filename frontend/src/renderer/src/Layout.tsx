import { FC, useState } from 'react'
import NavigationBar from './components/NavigationBar'
import { Container } from 'react-bootstrap'
import { ProblemSetMetadata } from './data/metadata'
import ProblemSet from './components/ProblemSet'

const Layout: FC = () => {
  const [selectedProblemSet, setSelectedProblemSet] = useState<ProblemSetMetadata | undefined>(undefined)

  return (
    <div className="m-1">
      <NavigationBar setProblemSet={setSelectedProblemSet} />

      {selectedProblemSet == null ? null : (
        <ProblemSet metadata={selectedProblemSet} />
      )}
    </div>

  )
}

export default Layout
