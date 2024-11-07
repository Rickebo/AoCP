import { FC, useState } from 'react'
import NavigationBar from './components/NavigationBar'
import { ProblemSetMetadata } from './data/metadata'
import ProblemSet from './components/ProblemSet'

const Layout: FC = () => {
  const [year, setYear] = useState<number | undefined>()
  const [selectedProblemSet, setSelectedProblemSet] = useState<ProblemSetMetadata | undefined>(undefined)

  return (
    <div className="m-1">
      <NavigationBar
        setProblemSet={(year, set) => {
          setYear(year)
          setSelectedProblemSet(set)
        }}
      />

      {year == null || selectedProblemSet == null ? null : (
        <ProblemSet year={year} set={selectedProblemSet} />
      )}
    </div>

  )
}

export default Layout
