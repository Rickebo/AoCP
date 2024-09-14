import { MetadataService } from './services/MetadataService'

const baseUrl = 'http://localhost:20011'

export function useMetadataService(): MetadataService {
  return new MetadataService(baseUrl)
}
