import { ProblemMetadata } from "@renderer/data/metadata"
import { FC } from "react"
import { Button, Container } from "react-bootstrap"
import ProblemDescription from "./ProblemDescription"

export interface ProblemProps {
    metadata: ProblemMetadata
}


const Problem: FC<ProblemProps> = (props) => {

    return (
        <Container className="">
            <h5>{props.metadata.name}</h5>
            {/* <p>{props.metadata.description}</p> */}
            <ProblemDescription metadata={props.metadata} />
            <Button>test</Button>
        </Container>

    )
}

export default Problem