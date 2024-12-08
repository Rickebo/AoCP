import { ProblemMetadata } from "@renderer/data/metadata"
import { FC } from "react"
import "./ProblemDescription.css"

export interface ProblemDescriptionProps {
    metadata: ProblemMetadata
}

const ProblemDescription: FC<ProblemDescriptionProps> = (props) => {
    const innerHtml = props.metadata.description != null
        ? { __html: props.metadata.description }
        : undefined

    return (
        <div 
            dangerouslySetInnerHTML={innerHtml} 
            className="problem"
            style={{
                fontFamily: 'Source Code Pro, monospace',
                fontSize: '1em',
                fontWeight: 'normal'
            }} 
        />
    )
}

export default ProblemDescription