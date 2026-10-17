import { useEffect, useState } from 'react';
import { getMenu, placeOrder } from '../api.js';

export default function MenuPage() {
  const [menu, setMenu] = useState([]);
  const [quantities, setQuantities] = useState({});
  const [employeeName, setEmployeeName] = useState('');
  const [message, setMessage] = useState(null);

  useEffect(() => {
    getMenu().then(setMenu).catch((e) => setMessage({ text: e.message, error: true }));
  }, []);

  function setQuantity(id, value) {
    setQuantities({ ...quantities, [id]: Math.max(0, Number(value) || 0) });
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const items = Object.entries(quantities)
      .filter(([, qty]) => qty > 0)
      .map(([id, qty]) => ({ menuItemId: Number(id), quantity: qty }));
    try {
      const order = await placeOrder(employeeName, items);
      setMessage({ text: `Order #${order.id} placed. Total: Rs ${order.total}`, error: false });
    } catch (err) {
      setMessage({ text: err.message, error: true });
    }
  }

  return (
    <section>
      <h2>Today's menu</h2>
      {message && (
        <div className={message.error ? 'message error' : 'message'}>{message.text}</div>
      )}
      <form onSubmit={handleSubmit}>
        <label>
          Your name
          <input value={employeeName} onChange={(e) => setEmployeeName(e.target.value)} />
        </label>
        {menu.map((item) => (
          <label key={item.id}>
            {item.name} - Rs {item.price} {!item.available && <span className="muted">(not available)</span>}
            <input
              type="number"
              min="0"
              disabled={!item.available}
              value={quantities[item.id] ?? 0}
              onChange={(e) => setQuantity(item.id, e.target.value)}
            />
          </label>
        ))}
        <button type="submit">Place order</button>
      </form>
    </section>
  );
}
