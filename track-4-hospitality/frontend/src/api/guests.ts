import { getJson } from './client'
import type { Guest, Paged } from './types'

export const listGuests = (params: { q?: string; page?: number }) =>
  getJson<Paged<Guest>>('/api/guests', { ...params, pageSize: 25 })
