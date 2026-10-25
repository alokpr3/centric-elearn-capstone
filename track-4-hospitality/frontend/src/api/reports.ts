import { getJson } from './client'
import type { OccupancyReport } from './types'

export const getOccupancy = (params: { hotelId?: number; from: string; to: string }) =>
  getJson<OccupancyReport>('/api/reports/occupancy', params)
