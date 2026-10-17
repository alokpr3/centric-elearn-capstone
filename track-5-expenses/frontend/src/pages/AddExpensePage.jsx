import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { addExpense } from '../api.js';

export default function AddExpensePage() {
  const navigate = useNavigate();
  const [description, setDescription] = useState('');
  const [paidBy, setPaidBy] = useState('');
  const [amount, setAmount] = useState('');
  const [people, setPeople] = useState('');
  const [error, setError] = useState('');

  async function handleSubmit(e) {
    e.preventDefault();
    setError('');
    try {
      await addExpense({
        description,
        paidBy,
        amount: Number(amount),
        sharedWith: people,
      });
      navigate('/');
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section>
      <h2>Add an expense</h2>
      {error && <div className="message error">{error}</div>}
      <form onSubmit={handleSubmit}>
        <label>
          What was it for?
          <input value={description} onChange={(e) => setDescription(e.target.value)} />
        </label>
        <label>
          Paid by
          <input value={paidBy} onChange={(e) => setPaidBy(e.target.value)} />
        </label>
        <label>
          Amount
          <input type="number" step="0.01" value={amount} onChange={(e) => setAmount(e.target.value)} />
        </label>
        <label>
          Shared with (names, separated by commas)
          <input value={people} onChange={(e) => setPeople(e.target.value)} />
        </label>
        <button type="submit">Save expense</button>
      </form>
    </section>
  );
}
