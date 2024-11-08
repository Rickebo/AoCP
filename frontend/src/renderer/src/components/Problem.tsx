import { ProblemMetadata } from '@renderer/data/metadata'
import { FC } from 'react'
import { Container } from 'react-bootstrap'
import ProblemDescription from './ProblemDescription'

export interface ProblemProps {
  metadata: ProblemMetadata
}

const Problem: FC<ProblemProps> = (props) => {
  return (
    <Container className="">
      {/* <p>{props.metadata.description}</p> */}
      <ProblemDescription metadata={props.metadata} />
    </Container>
  )
}

export default Problem
