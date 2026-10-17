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

export const getMenu = () => request('/menu');

export const getOrders = () => request('/orders');

export const placeOrder = (employeeName, items) =>
  request('/orders', {
    method: 'POST',
    body: JSON.stringify({ employeeName, items }),
  });
