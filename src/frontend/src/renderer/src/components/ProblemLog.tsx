import { FC } from 'react'

export interface ProblemLogProps {
  content: string[]
}

const ProblemLog: FC<ProblemLogProps> = (props) => {
  const lastIndex = props.content?.length ?? 0 - 1

  return (
    <pre
      className="h-100 w-100 mb-0 p-3"
      style={{
        fontFamily: 'Source Code Pro',
        fontWeight: 300,
        whiteSpace: 'pre-wrap'
      }}
    >
      <code>
        {props.content?.map((line, i) => (
          <span key={i}>
            {line}
            {i == lastIndex ? null : <br />}
          </span>
        ))}
      </code>
    </pre>
  )
}

export default ProblemLog
