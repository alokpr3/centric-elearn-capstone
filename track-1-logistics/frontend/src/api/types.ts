export interface Depot {
  id: number
  code: string
  name: string
  regionCode: string
  city: string
  dailyCapacity: number
  isActive: boolean
}

export interface Customer {
  id: number
  accountNo: string
  name: string
  contactName: string
  email: string
  phone: string
  city: string
  postcode: string
  regionCode: string
  isActive: boolean
}

export interface Paged<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export interface ConsignmentListItem {
  id: number
  reference: string
  customerName: string
  originDepot: string
  destDepot: string
  serviceLevel: string
  status: string
  bookedAt: string
  promisedBy: string
  deliveredAt: string | null
  driverName: string | null
}

export interface ConsignmentDetail extends ConsignmentListItem {
  parcels: number
  weightKg: number
  recipientName: string
  deliveryAddress: string
  deliveryPostcode: string
}

export interface Note {
  id: number
  noteType: string
  note: string
  createdAt: string
}

export interface DispatchOnTimeRow {
  depotId: number
  depotCode: string
  depotName: string
  total: number
  onTime: number
  late: number
  onTimePct: number
}

export interface DispatchOnTimeReport {
  total: number
  onTime: number
  late: number
  onTimePct: number
  depots: DispatchOnTimeRow[]
}

export interface ImportResult {
  batchId: number
  totalRows: number
  importedRows: number
  skippedRows: number
  messages: string[]
}
