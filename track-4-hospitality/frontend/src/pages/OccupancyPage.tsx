import { useState } from 'react'
import { getOccupancy } from '../api/reports'
import StateView from '../components/StateView'
import { useAsync } from '../hooks/useAsync'

export default function OccupancyPage() {
  const [from, setFrom] = useState('2026-10-01')
  const [to, setTo] = useState('2026-10-07')
  const report = useAsync(() => getOccupancy({ from, to }), [from, to])

  return (
    <section>
      <h1>Occupancy</h1>
      <div className="filters">
        <label>From <input type="date" value={from} onChange={e => setFrom(e.target.value)} /></label>
        <label>To <input type="date" value={to} onChange={e => setTo(e.target.value)} /></label>
      </div>
      <StateView state={report} isEmpty={d => d.days.length === 0} emptyText="No data for that range.">
        {data => (
          <>
            <p className="summary">
              {data.occupancyPct}% occupancy - {data.roomNightsSold} room nights sold across {data.roomsAvailable} rooms
            </p>
            <table>
              <thead><tr><th>Date</th><th>Rooms sold</th></tr></thead>
              <tbody>
                {data.days.map(d => <tr key={d.date}><td>{d.date}</td><td>{d.roomsSold}</td></tr>)}
              </tbody>
            </table>
          </>
        )}
      </StateView>
    </section>
  )
}
