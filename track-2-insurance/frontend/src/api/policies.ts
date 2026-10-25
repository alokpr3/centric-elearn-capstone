import { http } from './http';
import type { Paged, Policy, Policyholder, Product } from './types';

export const policiesApi = {
  list: (page = 1) => http.get<Paged<Policy>>(`/api/policies?page=${page}&pageSize=25`),
};

export const policyholdersApi = {
  list: (search: string, page = 1) =>
    http.get<Paged<Policyholder>>(`/api/policyholders?page=${page}&pageSize=25&search=${encodeURIComponent(search)}`),
};

export const productsApi = {
  list: () => http.get<Product[]>('/api/products'),
};
