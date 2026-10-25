import { useState } from 'react'
import { listBookings } from '../api/bookings'
import StateView from '../components/StateView'
import { useAsync } from '../hooks/useAsync'

const STATUSES = ['', 'Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled', 'NoShow']

export default function BookingsPage() {
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const bookings = useAsync(() => listBookings({ status: status || undefined, page }), [status, page])

  return (
    <section>
      <h1>Bookings</h1>
      <label>
        Status{' '}
        <select value={status} onChange={e => { setStatus(e.target.value); setPage(1) }}>
          {STATUSES.map(s => <option key={s} value={s}>{s || 'All'}</option>)}
        </select>
      </label>
      <StateView state={bookings} isEmpty={d => d.items.length === 0} emptyText="No bookings match.">
        {data => (
          <>
            <table>
              <thead>
                <tr><th>Ref</th><th>Hotel</th><th>Guest</th><th>Room type</th><th>Room</th><th>In</th><th>Out</th><th>Status</th><th>Total</th></tr>
              </thead>
              <tbody>
                {data.items.map(b => (
                  <tr key={b.id}>
                    <td>{b.reference}</td><td>{b.hotelName}</td><td>{b.guestName}</td><td>{b.roomTypeName}</td>
                    <td>{b.roomNumber ?? '-'}</td><td>{b.checkIn}</td><td>{b.checkOut}</td><td>{b.status}</td>
                    <td>{b.totalAmount.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            <div className="pager">
              <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
              <span>Page {data.page} of {Math.max(1, Math.ceil(data.total / data.pageSize))}</span>
              <button disabled={page * data.pageSize >= data.total} onClick={() => setPage(page + 1)}>Next</button>
            </div>
          </>
        )}
      </StateView>
    </section>
  )
}
