import type { CommunityEvent } from '../domain/CommunityEvent'

export interface EventRepository {
  list(signal: AbortSignal): Promise<CommunityEvent[]>
}

export function listEvents(repository: EventRepository, signal: AbortSignal) {
  return repository.list(signal)
}
