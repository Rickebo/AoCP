import { MetadataService } from './services/MetadataService'
import ProblemService from './services/ProblemService'

const baseUrl = 'http://localhost:20011'

export function useMetadataService(): MetadataService {
  return new MetadataService(baseUrl)
}


export function useProblemService(): ProblemService {
  return new ProblemService(baseUrl)
}
