import { createRef, CSSProperties, FC, ReactNode, useEffect, useRef, useState } from 'react'
import Problem from './Problem'
import ProblemInput from './ProblemInput'
import { ProblemSetMetadata } from '../data/metadata'
import ProblemSolution from './ProblemSolution'
import { Accordion, Col, Nav, Row, Stack, Tab, Tabs } from 'react-bootstrap'
import ProblemLog from './ProblemLog'
import { useConnectionManager } from '../ConnectionManager'
import classNames from 'classnames'
import Grid, { GridRef } from './Grid'
import ProblemDescription from './ProblemDescription'

export interface ProblemSetProps {
  year: number
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
            <h3 className={classNames('d-inline', props.textClassName)} style={props.textStyle}>
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
  const mgr = useConnectionManager(props.year, props.set, grids)

  const navigate = (url: string): void => {
    window.open(url)
  }

  const defaultKey = props.set.problems[0].name ?? undefined

  return (
    <div className="d-flex flex-column overflow-auto">
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

        <ProblemInput className="mb-3" onSolve={mgr.solveAll} />
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
        <Tab.Content>
          {props.set.problems.map((problem, i) => (
            <Tab.Pane
              key={problem.name}
              eventKey={problem.name ?? i.toString()}
              title={problem.name}
            >
              <div className="mx-1 mt-2 overflow-auto">
                <Tabs className="position-relative inset-0">
                  <Tab
                    key="desc"
                    title="Description"
                    className="d-flex flex-grow-1"
                    eventKey="desc"
                  >
                    <ProblemDescription metadata={problem} />
                  </Tab>
                  {problem.name == null ? null : (
                    <Tab key="grid" title="Grid" eventKey="grid">
                      <Grid
                        ref={(grid) => {
                          grids.current[problem.name] = grid
                        }}
                        displayHeight={400}
                        displayWidth={400}
                        width={100}
                        height={100}
                      />
                    </Tab>
                  )}
                  {problem.name == null ? null : (
                    <Tab key="log" title="Log" eventKey="log">
                      <ProblemLog content={mgr.log(problem.name)!} />
                    </Tab>
                  )}
                  {problem.name == null ? null : (
                    <Tab key="solution" title="Solution" eventKey="solution">
                      <ProblemSolution
                        key={problem.name}
                        problem={problem}
                        solution={mgr.solution(problem.name!)}
                        className="mt-3"
                      />
                    </Tab>
                  )}
                </Tabs>
              </div>
            </Tab.Pane>
          ))}
        </Tab.Content>
      </Tab.Container>
    </div>
  )
}

export default ProblemSet
