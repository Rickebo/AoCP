export interface Metadata {
  collections: ProblemCollectionMetadata[]
}

export interface ProblemCollectionMetadata {
  source: string
  year: number
  problemSets: Record<string, ProblemSetMetadata[]>
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
