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

export const getTickets = () => request('/tickets');

export const createTicket = (ticket) =>
  request('/tickets', {
    method: 'POST',
    body: JSON.stringify(ticket),
  });
