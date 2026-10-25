import { useState } from 'react'
import { listGuests } from '../api/guests'
import StateView from '../components/StateView'
import { useAsync } from '../hooks/useAsync'

export default function GuestsPage() {
  const [q, setQ] = useState('')
  const guests = useAsync(() => listGuests({ q: q || undefined }), [q])
  return (
    <section>
      <h1>Guests</h1>
      <label>
        Search{' '}
        <input value={q} onChange={e => setQ(e.target.value)} placeholder="name or email" />
      </label>
      <StateView state={guests} isEmpty={d => d.items.length === 0} emptyText="No guests match.">
        {data => (
          <table>
            <thead>
              <tr><th>Name</th><th>Email</th><th>Phone</th><th>City</th><th>Tier</th></tr>
            </thead>
            <tbody>
              {data.items.map(g => (
                <tr key={g.id}>
                  <td>{g.firstName} {g.lastName}</td><td>{g.email}</td><td>{g.phone}</td><td>{g.city}</td><td>{g.loyaltyTier}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </StateView>
    </section>
  )
}
