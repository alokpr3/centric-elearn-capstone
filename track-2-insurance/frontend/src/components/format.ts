export const money = (value: number | null | undefined) =>
  value == null ? '-' : value.toLocaleString('en-GB', { style: 'currency', currency: 'GBP' });
