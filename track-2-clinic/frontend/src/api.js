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

export const getDoctors = () => request('/doctors');

export const bookAppointment = (booking) =>
  request('/appointment', {
    method: 'POST',
    body: JSON.stringify(booking),
  });
