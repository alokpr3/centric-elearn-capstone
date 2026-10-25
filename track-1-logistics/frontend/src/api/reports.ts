import { apiGet, apiUpload } from './client'
import type { DispatchOnTimeReport, ImportResult } from './types'

export const dispatchOnTime = (from: string, to: string) =>
  apiGet<DispatchOnTimeReport>(`/reports/dispatch-on-time?from=${from}&to=${to}`)

export const importManifest = (file: File) => apiUpload<ImportResult>('/imports/manifest', file)
