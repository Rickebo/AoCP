import { FC, useState } from 'react'
import NavigationBar from './components/NavigationBar'
import { ProblemSetMetadata } from './data/metadata'
import ProblemSet from './components/ProblemSet'
import { usePersistentState } from './StateUtils'

interface AuthorState {
  author: string
}

const Layout: FC = () => {
  const [year, setYear] = useState<number | undefined>()

  const authorState = usePersistentState<AuthorState>('author', {
    author: 'Unknown'
  })

  const [selectedProblemSet, setSelectedProblemSet] = useState<ProblemSetMetadata | undefined>(
    undefined
  )

  const author = authorState.state.author
  const setAuthor = (newAuthor: string): void => {
    authorState.update((current) => {
      current.author = newAuthor
      authorState.save(current)
    })

    // setSelectedProblemSet(undefined)
  }

  return (
    <div className="h-100 d-flex flex-column">
      <NavigationBar
        setProblemSet={(year, set) => {
          setYear(year)
          setSelectedProblemSet(set)
        }}
        author={author ?? 'Unknown'}
        setAuthor={setAuthor}
      />

      <div className="d-flex flex-column flex-grow-1 overflow-hidden">
        {year == null || author == null || selectedProblemSet == null ? null : (
          <ProblemSet
            key={`problem-${year}-${author}-${selectedProblemSet.name}`}
            year={year}
            author={author}
            set={selectedProblemSet}
          />
        )}
      </div>
    </div>
  )
}

export default Layout
