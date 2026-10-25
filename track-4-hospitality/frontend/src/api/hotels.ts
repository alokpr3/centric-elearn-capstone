import { getJson } from './client'
import type { Hotel } from './types'

export const listHotels = () => getJson<Hotel[]>('/api/hotels')
