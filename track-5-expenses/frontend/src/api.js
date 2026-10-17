async function request(path, options = {}) {
  const res = await fetch(`/api${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  const body = await res.json().catch(() => null);
  if (!res.ok) {
    throw new Error(body?.message || `Request failed (${res.status})`);
  }
  return body;
}

export const getExpenses = () => request('/expenses');

export const getSplit = (id) => request(`/expenses/${id}/split`);

export const addExpense = (expense) =>
  request('/expenses', {
    method: 'POST',
    body: JSON.stringify(expense),
  });
