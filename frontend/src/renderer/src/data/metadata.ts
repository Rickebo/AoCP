export interface Metadata {
  collections: Record<number, ProblemCollectionMetadata>
}

export interface ProblemCollectionMetadata {
  year: number
  problemSets: ProblemSetMetadata[]
}

export interface ProblemSetMetadata {
  releaseTime: string
  problems: ProblemMetadata[]
}

export interface ProblemMetadata {
  name: string
}
