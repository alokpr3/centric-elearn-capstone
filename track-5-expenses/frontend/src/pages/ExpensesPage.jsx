import { useEffect, useState } from 'react';
import { getExpenses, getSplit } from '../api.js';

export default function ExpensesPage() {
  const [expenses, setExpenses] = useState([]);
  const [selected, setSelected] = useState(null);
  const [split, setSplit] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    getExpenses().then(setExpenses).catch((e) => setError(e.message));
  }, []);

  async function showSplit(expense) {
    setSelected(expense);
    try {
      setSplit(await getSplit(expense.id));
    } catch (e) {
      setError(e.message);
    }
  }

  return (
    <section>
      <h2>Expenses</h2>
      {error && <div className="message error">{error}</div>}
      <table>
        <thead>
          <tr>
            <th>What</th>
            <th>Paid by</th>
            <th>Amount</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {expenses.map((e) => (
            <tr key={e.id}>
              <td>{e.description}</td>
              <td>{e.paidBy}</td>
              <td>{e.amount}</td>
              <td>
                <button onClick={() => showSplit(e)}>Show split</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {selected && (
        <div className="card">
          <h3>Split for {selected.description}</h3>
          <table>
            <thead>
              <tr>
                <th>Person</th>
                <th>Share</th>
                <th>Owes {selected.paidBy}</th>
              </tr>
            </thead>
            <tbody>
              {split.map((s) => (
                <tr key={s.person}>
                  <td>{s.person}</td>
                  <td>{s.share}</td>
                  <td>{s.owes}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  );
}
