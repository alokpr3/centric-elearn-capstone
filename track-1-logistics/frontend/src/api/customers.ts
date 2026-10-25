import { apiGet } from './client'
import type { Customer, Paged } from './types'

export const searchCustomers = (search: string, page = 1) =>
  apiGet<Paged<Customer>>(`/customers?search=${encodeURIComponent(search)}&page=${page}&pageSize=25`)
