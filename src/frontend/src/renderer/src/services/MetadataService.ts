import { Metadata } from '../data/metadata'

export class MetadataService {
  private baseUrl: string

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl
  }

  async getMetadata(): Promise<Metadata> {
    const response = await fetch(this.baseUrl + '/metadata')
    return (await response.json()) as Metadata
  }
}
