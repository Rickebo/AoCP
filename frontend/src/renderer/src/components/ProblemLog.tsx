import { FC } from 'react'

export interface ProblemLogProps {
  content: string[]
}

const ProblemLog: FC<ProblemLogProps> = (props) => {
  const lastIndex = props.content?.length ?? 0 - 1

  return (
    <div className="bg-body-tertiary p-2 px-3 rounded">
      <pre
        style={{
          fontFamily: 'Source Code Pro',
          fontWeight: 300
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
    </div>
  )
}

export default ProblemLog
