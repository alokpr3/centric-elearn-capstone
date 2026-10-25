import { apiGet, apiSend } from './client'
import type { ConsignmentDetail, ConsignmentListItem, Note, Paged } from './types'

export const searchConsignments = (status: string, search: string, page = 1) => {
  const params = new URLSearchParams({ page: String(page), pageSize: '25' })
  if (status) params.set('status', status)
  if (search) params.set('search', search)
  return apiGet<Paged<ConsignmentListItem>>(`/consignments?${params}`)
}

export const getConsignment = (id: number) => apiGet<ConsignmentDetail>(`/consignments/${id}`)
export const listNotes = (id: number) => apiGet<Note[]>(`/consignments/${id}/notes`)
export const listStatuses = () => apiGet<string[]>('/statuses')
export const cancelConsignment = (id: number, reason: string) =>
  apiSend<ConsignmentDetail>('POST', `/consignments/${id}/cancel`, { reason })
