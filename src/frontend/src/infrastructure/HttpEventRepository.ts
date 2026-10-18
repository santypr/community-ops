import type { EventRepository } from '../application/EventRepository'
import type { CommunityEvent } from '../domain/CommunityEvent'

function isEvent(value: unknown): value is CommunityEvent {
  if (typeof value !== 'object' || value === null) return false
  const event = value as Record<string, unknown>
  return ['id', 'communityId', 'title', 'startsAt', 'endsAt', 'location'].every(
    (key) => typeof event[key] === 'string',
  ) && !Number.isNaN(Date.parse(event.startsAt as string))
    && !Number.isNaN(Date.parse(event.endsAt as string))
}

export class HttpEventRepository implements EventRepository {
  private readonly baseUrl: string

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl
  }

  async list(signal: AbortSignal): Promise<CommunityEvent[]> {
    const response = await fetch(`${this.baseUrl.replace(/\/$/, '')}/api/events`, {
      signal,
      headers: { Accept: 'application/json' },
    })
    if (!response.ok) throw new Error('No se pudieron cargar los eventos.')
    const data: unknown = await response.json()
    if (!Array.isArray(data) || !data.every(isEvent)) {
      throw new Error('La respuesta del servicio no tiene el formato esperado.')
    }
    return data
  }
}
