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

export const getBooks = () => request('/books');

export const borrowBook = (bookId, borrower) =>
  request(`/books/${bookId}/borrow`, {
    method: 'POST',
    body: JSON.stringify({ borrower }),
  });
