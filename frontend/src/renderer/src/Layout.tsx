import { FC, useState } from 'react'
import NavigationBar from './components/NavigationBar'
import { ProblemSetMetadata } from './data/metadata'
import ProblemSet from './components/ProblemSet'

const Layout: FC = () => {
  const [year, setYear] = useState<number | undefined>()
  const [selectedProblemSet, setSelectedProblemSet] = useState<ProblemSetMetadata | undefined>(
    undefined
  )

  return (
    <div className="h-100 d-flex flex-column">
      <NavigationBar
        setProblemSet={(year, set) => {
          setYear(year)
          setSelectedProblemSet(set)
        }}
      />

      <div className="d-flex flex-column flex-grow-1 overflow-hidden">
        {year == null || selectedProblemSet == null ? null : (
          <ProblemSet year={year} set={selectedProblemSet} />
        )}
      </div>
    </div>
  )
}

export default Layout
