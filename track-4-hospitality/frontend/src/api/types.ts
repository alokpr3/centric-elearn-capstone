export interface Hotel {
  id: number
  code: string
  name: string
  city: string
  region: string
  addressLine: string
  phone: string
  active: boolean
}

export interface Paged<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface Booking {
  id: number
  reference: string
  channel: string
  hotelName: string
  guestName: string
  roomTypeName: string
  roomNumber: string | null
  checkIn: string
  checkOut: string
  nights: number
  status: string
  totalAmount: number
}

export interface Guest {
  id: number
  firstName: string
  lastName: string
  email: string
  phone: string
  city: string
  region: string
  loyaltyTier: string
}

export interface OccupancyReport {
  hotelId: number | null
  from: string
  to: string
  roomsAvailable: number
  roomNightsSold: number
  occupancyPct: number
  days: { date: string; roomsSold: number }[]
}
