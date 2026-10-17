import { useEffect, useState } from 'react';
import { getOrders } from '../api.js';

export default function OrdersPage() {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    getOrders().then(setOrders);
  }, []);

  return (
    <section>
      <h2>Orders</h2>
      {orders.length === 0 && <p className="muted">No orders yet.</p>}
      <table>
        <thead>
          <tr>
            <th>#</th>
            <th>Employee</th>
            <th>Items</th>
            <th>Total (Rs)</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((o) => (
            <tr key={o.id}>
              <td>{o.id}</td>
              <td>{o.employeeName}</td>
              <td>{o.lines.length}</td>
              <td>{o.total}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
