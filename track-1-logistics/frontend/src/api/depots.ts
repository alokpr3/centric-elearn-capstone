import { apiGet } from './client'
import type { Depot } from './types'

export const listDepots = (region?: string) =>
  apiGet<Depot[]>(`/depots${region ? `?region=${encodeURIComponent(region)}` : ''}`)
