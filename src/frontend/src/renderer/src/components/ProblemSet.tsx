import React, { CSSProperties, FC, ReactNode, useEffect, useRef, useState } from 'react'
import ProblemInput from './ProblemInput'
import { ProblemSetMetadata } from '../data/metadata'
import { Col, Nav, Stack, Tab } from 'react-bootstrap'
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

  const isSolving = (problemName: string | undefined): boolean => {
    const data = mgr.getSolveData(problemName)
    if (data == null || data.end != null) return false

    const elapsed = new Date().getTime() - data.start.getTime()
    return elapsed > startCooldown
  }

  const solveElapsedTime = (problemName: string | undefined): string =>
    mgr.elapsed(problemName) ?? ''

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

    return (): void => {
      clearInterval(interval)
      updater()
    }
  }, [...props.set.problems.map((prob) => mgr.elapsed(prob.name))])

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

      <Stack direction="horizontal">
        {props.set.problems.map((problem, i) => (
          <Col key={i}>
            <Stack direction="horizontal">
              <span
                className="ms-3"
                style={{
                  fontWeight: 500
                }}
              >
                {problem.name}
              </span>
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
              {problem.name == null || mgr.elapsed(problem.name) == null ? null : (
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
                  {mgr.elapsed(problem.name)}
                </span>
              )}
            </Stack>
          </Col>
        ))}
      </Stack>

      <Tab.Container defaultActiveKey={`desc-0`}>
        <Nav variant="tabs">
          <Nav.Item eventKey="desc"></Nav.Item>

          {props.set.problems.map((problem, i) => (
            <Col key={i}>
              <Stack direction="horizontal">
                <Nav.Item>
                  <Nav.Link eventKey={`desc-${i}`}>Description</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                  <Nav.Link eventKey={`grid-${i}`}>Grid</Nav.Link>
                </Nav.Item>
                <Nav.Item>
                  <Nav.Link eventKey={`log-${i}`}>Log</Nav.Link>
                </Nav.Item>
              </Stack>
            </Col>
          ))}
        </Nav>
        <Tab.Content
          style={{ display: 'flex', flex: '1 1 auto' }}
          className="h-100 w-100 overflow-auto"
        >
          {props.set.problems.map((problem, i) => (
            <>
              <Tab.Pane eventKey={`desc-${i}`}>
                <ProblemDescription metadata={problem} />
              </Tab.Pane>
              <Tab.Pane
                eventKey={`grid-${i}`}
                style={{
                  flexGrow: '1'
                }}
              >
                <div className="w-100 h-100 d-flex">
                  {problem.name == null ? null : (
                    <Grid
                      ref={(grid) => {
                        grids.current[problem.name] = grid
                      }}
                    />
                  )}
                </div>
              </Tab.Pane>
              <Tab.Pane
                eventKey={`log-${i}`}
                style={{
                  flexGrow: '1'
                }}
              >
                <div className="w-100 h-100 d-flex flex">
                  {problem.name == null ? null : <ProblemLog content={mgr.log(problem.name)!} />}
                </div>
              </Tab.Pane>
            </>
          ))}
        </Tab.Content>
      </Tab.Container>
    </div>
  )
}

export default ProblemSet
