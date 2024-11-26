import { createRef, CSSProperties, FC, ReactNode, useEffect, useRef } from 'react'
import Problem from './Problem'
import ProblemInput from './ProblemInput'
import { ProblemSetMetadata } from '../data/metadata'
import ProblemSolution from './ProblemSolution'
import { Accordion, Tab, Tabs } from 'react-bootstrap'
import ProblemLog from './ProblemLog'
import { useConnectionManager } from '../ConnectionManager'
import classNames from 'classnames'
import Grid, { GridRef } from './Grid'

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
  const gridRef = useRef<GridRef>(null)
  const mgr = useConnectionManager(props.year, props.set, gridRef)

  const navigate = (url: string): void => {
    window.open(url)
  }

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

      <Tabs>
        {props.set.problems.map((problem, i) => (
          <Tab key={problem.name} eventKey={problem.name ?? i.toString()} title={problem.name}>
            <div className="mx-1 mt-2 overflow-auto">
              <Problem key={problem.name} metadata={problem} />

              {problem.name == null || mgr.log(problem.name) == null ? null : (
                <Accordion className="mt-3">
                  <Accordion.Item eventKey="log" title="Log">
                    <Accordion.Header>Log</Accordion.Header>
                    <Accordion.Body>
                      <ProblemLog content={mgr.log(problem.name)!} />
                    </Accordion.Body>
                  </Accordion.Item>
                </Accordion>
              )}

              <Grid
                ref={gridRef}
                displayHeight={400}
                displayWidth={400}
                width={100}
                height={100}
              />

              {problem.name == null || mgr.solution(problem.name) == null ? null : (
                <ProblemSolution
                  key={problem.name}
                  problem={problem}
                  solution={mgr.solution(problem.name!)}
                  className="mt-3"
                />
              )}
            </div>
          </Tab>
        ))}
      </Tabs>
    </div>
  )
}

export default ProblemSet
