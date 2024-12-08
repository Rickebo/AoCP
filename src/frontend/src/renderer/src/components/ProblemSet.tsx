import React, { CSSProperties, FC, ReactNode, useEffect, useRef, useState } from 'react'
import ProblemInput from './ProblemInput'
import { ProblemSetMetadata } from '../data/metadata'
import { Nav, Spinner, Stack, Tab } from 'react-bootstrap'
import ProblemLog from './ProblemLog'
import { useConnectionManager } from '../ConnectionManager'
import classNames from 'classnames'
import Grid, { GridRef } from './Grid'
import ProblemDescription from './ProblemDescription'
import { BsCheck2Square, BsCopy, BsStopwatch } from 'react-icons/bs'

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
        <React.Fragment key={`title-part-${i}`}>
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
        </React.Fragment>
      ))}
    </div>
  )
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
  const grids = useRef<Record<string, GridRef>>({})
  const mgr = useConnectionManager(props.year, props.author, props.set, grids)
  const [solveData, setSolveData] = useState<Record<string, string>>({})

  // Cooldown before showing spinner for problem being solved. To prevent it from flashing
  // too quickly when a solution is just a bit too efficient.
  const startCooldown = 10

  const navigate = (url: string): void => {
    window.open(url)
  }

  const defaultKey = props.set.problems[0].name ?? undefined

  const isSolving = (problemName: string | undefined): boolean => {
    const data = mgr.getSolveData(problemName)
    if (data == null || data.end != null) return false

    const elapsed = new Date().getTime() - data.start.getTime()
    return elapsed > startCooldown
  }

  const solveElapsedTime = (problemName: string | undefined): string => {
    const data = mgr.getSolveData(problemName)
    if (data == null) return ''

    const start = data.start
    const end = data.end ?? new Date()

    const elapsed = end.getTime() - start.getTime()
    const seconds = Math.floor(elapsed / 1000)
    const minutes = Math.floor(seconds / 60)

    if (minutes > 0)
      return `${minutes}m ${(seconds % 60).toString().padStart(2, '0')}s ${(elapsed % 1000).toString().padStart(3, '0')} ms`
    else if (seconds > 0) return `${seconds}s ${(elapsed % 1000).toString().padStart(3, '0')} ms`
    else return `${elapsed} ms`
  }

  const isSolvingAny = props.set.problems
    .map((problem) => isSolving(problem.name))
    .reduce((a, b) => a || b, false)

  useEffect(() => {
    if (!isSolvingAny) return

    const updater = (): void => {
      const newData = Object.fromEntries(
        props.set.problems.map((problem) => [problem.name, solveElapsedTime(problem.name)])
      )
      setSolveData(newData)
    }

    updater()
    const interval = setInterval(updater, 7)

    return (): void => clearInterval(interval)
  }, [isSolvingAny])

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
          year={props.year}
          day={new Date(props.set.releaseTime).getDate()}
        />
      </div>

      <Tab.Container id="problems" defaultActiveKey={defaultKey}>
        <Stack direction="horizontal" className="mx-1">
          <Nav variant="pills">
            {props.set.problems.map((problem, i) => (
              <Nav.Item key={problem.name}>
                <Nav.Link eventKey={problem.name ?? i.toString()}>
                  {problem.name}
                  {isSolving(problem.name) ? <Spinner size="sm" className="ms-2" /> : null}
                </Nav.Link>
              </Nav.Item>
            ))}
          </Nav>
          <div className="ms-auto" />
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

                    <div className="ms-auto" />
                    {problem.name == null || mgr.solution(problem.name) == null ? null : (
                      <span
                        style={{
                          opacity: 0.5,
                          alignItems: 'center',
                          display: 'flex',
                          color: 'lightgreen'
                        }}
                        className="me-4"
                      >
                        <BsCheck2Square className="me-2" />
                        {mgr.solution(problem.name)}
                        <div
                          style={{ alignItems: 'center', display: 'flex', cursor: 'pointer' }}
                          className="m-0"
                          onClick={() => {
                            const solution = mgr.solution(problem.name!)

                            if (solution != null) navigator.clipboard.writeText(solution)
                          }}
                        >
                          <BsCopy className="ms-2" />
                        </div>
                      </span>
                    )}
                    {problem.name == null || solveData[problem.name] == null ? null : (
                      <span
                        style={{
                          opacity: 0.5,
                          alignItems: 'center',
                          display: 'flex',
                          color: 'var(--bs-btn-bg)'
                        }}
                        className="me-3"
                      >
                        <BsStopwatch className="me-2 my-0" />
                        {solveData[problem.name]}
                      </span>
                    )}
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
