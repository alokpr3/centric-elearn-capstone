import { http } from './http';
import type { Claim, ClaimListItem, ClaimNote, Paged } from './types';

export const claimsApi = {
  list: (status?: string, page = 1) => {
    const params = new URLSearchParams({ page: String(page), pageSize: '25' });
    if (status) params.set('status', status);
    return http.get<Paged<ClaimListItem>>(`/api/claims?${params}`);
  },
  get: (id: number) => http.get<Claim>(`/api/claims/${id}`),
  notes: (id: number) => http.get<ClaimNote[]>(`/api/claims/${id}/notes`),
};
