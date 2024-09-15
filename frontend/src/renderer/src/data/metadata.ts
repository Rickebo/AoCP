export interface Metadata {
  collections: Record<number, ProblemCollectionMetadata>
}

export interface ProblemCollectionMetadata {
  year: number
  problemSets: ProblemSetMetadata[]
}

export interface ProblemSetMetadata {
  name: string
  releaseTime: string
  problems: ProblemMetadata[]
}

export interface ProblemMetadata {
  name?: string | undefined
  description?: string | undefined
}
