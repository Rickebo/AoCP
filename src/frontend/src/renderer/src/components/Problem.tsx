import { ProblemMetadata } from '@renderer/data/metadata'
import { FC } from 'react'
import { Accordion } from 'react-bootstrap'
import ProblemDescription from './ProblemDescription'

export interface ProblemProps {
  metadata: ProblemMetadata
  problemKey: string
  year: number
  day: number
  partIndex: number
}

const Problem: FC<ProblemProps> = (props) => {
  return (
    <div className="">
      {/* <p>{props.metadata.description}</p> */}
      <Accordion defaultActiveKey="description">
        <Accordion.Item eventKey="description">
          <Accordion.Header>Problem description</Accordion.Header>
          <Accordion.Body>
            <ProblemDescription
              metadata={props.metadata}
              problemKey={props.problemKey}
              year={props.year}
              day={props.day}
              partIndex={props.partIndex}
            />
          </Accordion.Body>
        </Accordion.Item>
      </Accordion>
    </div>
  )
}

export default Problem
