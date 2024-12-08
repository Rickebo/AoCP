import { MetadataService } from './services/MetadataService'
import ProblemService from './services/ProblemService'
import { useBackend } from './context/BackendContext'

export function useMetadataService(): MetadataService {
  const backend = useBackend()
  return new MetadataService(backend.url)
}

export function useProblemService(): ProblemService {
  const backend = useBackend()
  return new ProblemService(backend.url)
}
