import { useEffect, useState } from 'react';
import { borrowBook, getBooks } from '../api.js';

export default function BorrowPage() {
  const [books, setBooks] = useState([]);
  const [bookId, setBookId] = useState('');
  const [borrower, setBorrower] = useState('');
  const [message, setMessage] = useState(null);

  useEffect(() => {
    getBooks().then((list) => {
      setBooks(list);
      if (list.length > 0) setBookId(String(list[0].id));
    });
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      const loan = await borrowBook(bookId, borrower);
      const due = new Date(loan.dueDate).toLocaleDateString();
      setMessage({ text: `Borrowed. Please return by ${due}.`, error: false });
      setBorrower('');
      setBooks(await getBooks());
    } catch (err) {
      setMessage({ text: err.message, error: true });
    }
  }

  return (
    <section>
      <h2>Borrow a book</h2>
      {message && (
        <div className={message.error ? 'message error' : 'message'}>{message.text}</div>
      )}
      <form onSubmit={handleSubmit}>
        <label>
          Book
          <select value={bookId} onChange={(e) => setBookId(e.target.value)}>
            {books.map((b) => (
              <option key={b.id} value={b.id}>
                {b.title}
              </option>
            ))}
          </select>
        </label>
        <label>
          Your name
          <input value={borrower} onChange={(e) => setBorrower(e.target.value)} />
        </label>
        <button type="submit">Borrow</button>
      </form>
    </section>
  );
}
