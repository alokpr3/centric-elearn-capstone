import { getJson } from './client'
import type { Booking, Paged } from './types'

export const listBookings = (params: { hotelId?: number; status?: string; page?: number }) =>
  getJson<Paged<Booking>>('/api/bookings', { ...params, pageSize: 25 })
