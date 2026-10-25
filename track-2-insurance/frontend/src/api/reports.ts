import { http } from './http';
import type { LossRatioRow } from './types';

export const reportsApi = {
  lossRatio: (from: string, to: string) => http.get<LossRatioRow[]>(`/api/reports/loss-ratio?from=${from}&to=${to}`),
};
