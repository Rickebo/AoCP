import { ProblemSetMetadata } from "@renderer/data/metadata"
import { FC } from "react"
import Problem from "./Problem"

export interface ProblemSetProps {
    metadata: ProblemSetMetadata
}

const ProblemSet: FC<ProblemSetProps> = (props) => {
    return (
        <div>
            <h1>{props.metadata.name}</h1>
            {props.metadata.problems.map((problem) => (
                <Problem key={problem.name} metadata={problem} />
            ))}
        </div>
    )
}

export default ProblemSet