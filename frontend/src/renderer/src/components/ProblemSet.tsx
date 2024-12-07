import { CSSProperties, FC, ReactNode, useRef } from 'react'
import ProblemInput from './ProblemInput'
import { ProblemSetMetadata } from '../data/metadata'
import ProblemSolution from './ProblemSolution'
import { Nav, Stack, Tab } from 'react-bootstrap'
import ProblemLog from './ProblemLog'
import { useConnectionManager } from '../ConnectionManager'
import classNames from 'classnames'
import Grid, { GridRef } from './Grid'
import ProblemDescription from './ProblemDescription'

export interface ProblemSetProps {
  year: number
  author: string
  set: ProblemSetMetadata
}

export interface ProblemTitleProps {
  className?: string | undefined
  style?: CSSProperties | undefined
  textClassName?: string | undefined
  textStyle?: CSSProperties | undefined
  parts: ReactNode[]
  delimiter: ReactNode
}

const ProblemTitle: FC<ProblemTitleProps> = (props) => {
  return (
    <div className={classNames(props.className, 'd-inline')} style={props.style}>
      {props.parts.map((part, i) => (
        <>
          <h3
            key={i}
            className={classNames('d-inline', props.textClassName)}
            style={props.textStyle}
          >
            {part}
          </h3>
          {i == props.parts.length - 1 ? null : (
            <h3
              key={`${i}-separator`}
              className={classNames('d-inline', props.textClassName)}
              style={props.textStyle}
            >
              &nbsp;
              {props.delimiter}
              &nbsp;
            </h3>
          )}
        </>
      ))}
    </div>
  )
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
  const grids = useRef<Record<string, GridRef>>({})
  const mgr = useConnectionManager(props.year, props.author, props.set, grids)

  const navigate = (url: string): void => {
    window.open(url)
  }

  const defaultKey = props.set.problems[0].name ?? undefined

  return (
    <div className="d-flex flex-column overflow-auto" style={{ flex: '1 1 auto' }}>
      <div className="mx-2">
        <ProblemTitle
          delimiter={'/'}
          parts={[
            <a
              key="year"
              href="#"
              onClick={() => navigate(`https://adventofcode.com/${props.year}`)}
            >
              {props.year}
            </a>,
            <a
              key="day"
              href="#"
              onClick={() =>
                navigate(
                  `https://adventofcode.com/${props.year}/day/${new Date(props.set.releaseTime).getDate()}`
                )
              }
            >
              {props.set.name}
            </a>
          ]}
          textStyle={{
            fontFamily: 'Source Code Pro, monospace',
            fontWeight: 800
          }}
        />

        <ProblemInput
          className="mb-3"
          onSolve={mgr.solveAll}
          problemKey={`input-${props.year}-${props.set.name}`}
        />
      </div>

      <Tab.Container id="problems" defaultActiveKey={defaultKey}>
        <Stack direction="horizontal" className="mx-1">
          <Nav variant="pills">
            {props.set.problems.map((problem, i) => (
              <Nav.Item key={problem.name}>
                <Nav.Link eventKey={problem.name ?? i.toString()}>{problem.name}</Nav.Link>
              </Nav.Item>
            ))}
          </Nav>
        </Stack>
        <Tab.Content style={{ display: 'flex', flex: '1 1 auto', overflow: 'hidden' }}>
          {props.set.problems.map((problem, i) => (
            <Tab.Pane
              key={problem.name}
              eventKey={problem.name ?? i.toString()}
              title={problem.name}
              className="h-100 w-100"
            >
              <div
                className="d-flex-column flex-grow-1 h-100 w-100"
                style={{
                  display: 'flex',
                  flex: '1 1 auto',
                  flexFlow: 'column'
                }}
              >
                <Tab.Container>
                  <Nav variant="tabs">
                    <Nav.Item>
                      <Nav.Link eventKey="description">Description</Nav.Link>
                    </Nav.Item>
                    <Nav.Item>
                      <Nav.Link eventKey="grid">Grid</Nav.Link>
                    </Nav.Item>
                    <Nav.Item>
                      <Nav.Link eventKey="log">Log</Nav.Link>
                    </Nav.Item>
                    <Nav.Item>
                      <Nav.Link eventKey="solution">Solution</Nav.Link>
                    </Nav.Item>
                  </Nav>
                  <Tab.Content
                    style={{ display: 'flex', flex: '1 1 auto' }}
                    className="h-100 w-100 overflow-auto"
                  >
                    <Tab.Pane eventKey="description">
                      <ProblemDescription metadata={problem} />
                    </Tab.Pane>
                    <Tab.Pane
                      eventKey="grid"
                      style={{
                        flexGrow: '1'
                      }}
                    >
                      <div className="w-100 h-100 d-flex">
                        {problem.name == null ? null : (
                          <Grid
                            ref={(grid) => {
                              // eslint-disable-next-line @typescript-eslint/ban-ts-comment
                              // @ts-ignore
                              grids.current[problem.name] = grid
                            }}
                          />
                        )}
                      </div>
                    </Tab.Pane>
                    <Tab.Pane eventKey="log">
                      {problem.name == null ? null : (
                        <ProblemLog content={mgr.log(problem.name)!} />
                      )}
                    </Tab.Pane>
                    <Tab.Pane eventKey="solution">
                      {problem.name == null ? null : (
                        <ProblemSolution
                          key={problem.name}
                          problem={problem}
                          solution={mgr.solution(problem.name!)}
                          className="mt-3"
                        />
                      )}
                    </Tab.Pane>
                  </Tab.Content>
                </Tab.Container>
              </div>
            </Tab.Pane>
          ))}
        </Tab.Content>
      </Tab.Container>
    </div>
  )
}

export default ProblemSet
