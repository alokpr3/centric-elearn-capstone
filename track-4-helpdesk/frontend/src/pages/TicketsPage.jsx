import { useEffect, useState } from 'react';
import { createTicket, getTickets } from '../api.js';

const PRIORITIES = ['Low', 'Medium', 'High', 'Urgent'];

export default function TicketsPage() {
  const [tickets, setTickets] = useState([]);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [raisedBy, setRaisedBy] = useState('');
  const [priority, setPriority] = useState('Medium');
  const [message, setMessage] = useState(null);

  useEffect(() => {
    getTickets().then(setTickets).catch((e) => setMessage({ text: e.message, error: true }));
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      await createTicket({ title, description, raisedBy, priority });
      setMessage({ text: 'Ticket raised.', error: false });
      setTitle('');
      setDescription('');
    } catch (err) {
      setMessage({ text: err.message, error: true });
    }
  }

  return (
    <div className="stack">
      <section>
        <h2>Tickets</h2>
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Raised by</th>
              <th>Priority</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {tickets.map((t) => (
              <tr key={t.id}>
                <td>{t.title}</td>
                <td>{t.raisedBy}</td>
                <td>
                  <span className={`badge ${t.priority}`}>{t.priority}</span>
                </td>
                <td>{t.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section>
        <h2>Raise a ticket</h2>
        {message && (
          <div className={message.error ? 'message error' : 'message'}>{message.text}</div>
        )}
        <form onSubmit={handleSubmit}>
          <label>
            Title
            <input value={title} onChange={(e) => setTitle(e.target.value)} />
          </label>
          <label>
            Details
            <textarea value={description} onChange={(e) => setDescription(e.target.value)} />
          </label>
          <label>
            Your name
            <input value={raisedBy} onChange={(e) => setRaisedBy(e.target.value)} />
          </label>
          <label>
            Priority
            <select value={priority} onChange={(e) => setPriority(e.target.value)}>
              {PRIORITIES.map((p) => (
                <option key={p}>{p}</option>
              ))}
            </select>
          </label>
          <button type="submit">Raise ticket</button>
        </form>
      </section>
    </div>
  );
}
