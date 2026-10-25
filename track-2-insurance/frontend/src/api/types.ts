export interface Paged<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ClaimListItem {
  id: number;
  claimNumber: string;
  policyNumber: string;
  policyholderName: string;
  productName: string;
  handlerName: string | null;
  status: string;
  claimType: string;
  lossDate: string;
  estimatedAmount: number;
  noteCount: number;
  reopenedCount: number;
}

export interface Claim {
  id: number;
  claimNumber: string;
  policyId: number;
  handlerId: number | null;
  claimType: string;
  status: string;
  lossDate: string;
  reportedDate: string;
  description: string;
  estimatedAmount: number;
  approvedAmount: number | null;
  paidAmount: number | null;
  excess: number;
  reopenedCount: number;
  reserveReviewDue: boolean;
}

export interface ClaimNote {
  id: number;
  claimId: number;
  authorUserId: number;
  noteType: string;
  body: string;
  createdAt: string;
}

export interface Policy {
  id: number;
  policyNumber: string;
  policyholderName: string;
  productCode: string;
  productName: string;
  startDate: string;
  endDate: string;
  annualPremium: number;
  sumInsured: number;
  status: string;
}

export interface Policyholder {
  id: number;
  fullName: string;
  email: string;
  phone: string | null;
  city: string;
  postcode: string;
}

export interface Product {
  id: number;
  code: string;
  name: string;
  line: string;
  basePremium: number;
  defaultExcess: number;
  active: boolean;
}

export interface LossRatioRow {
  productCode: string;
  productName: string;
  writtenPremium: number;
  paidClaims: number;
  lossRatioPct: number;
  claimCount: number;
}
