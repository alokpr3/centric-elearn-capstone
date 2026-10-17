import { useEffect, useState } from 'react';
import { getBooks } from '../api.js';

export default function BooksPage() {
  const [books, setBooks] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    getBooks().then(setBooks).catch((e) => setError(e.message));
  }, []);

  return (
    <section>
      <h2>Books</h2>
      {error && <div className="message error">{error}</div>}
      <table>
        <thead>
          <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Copies available</th>
          </tr>
        </thead>
        <tbody>
          {books.map((book) => (
            <tr key={book.id}>
              <td>{book.title}</td>
              <td>{book.author}</td>
              <td>{book.copies}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
