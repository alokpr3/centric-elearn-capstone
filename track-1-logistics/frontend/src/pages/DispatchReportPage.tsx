import { useState } from 'react'
import { dispatchOnTime } from '../api/reports'
import AsyncView from '../components/AsyncView'
import { useApi } from '../hooks/useApi'

export default function DispatchReportPage() {
  const [from, setFrom] = useState('2026-09-01')
  const [to, setTo] = useState('2026-09-30')
  const state = useApi(() => dispatchOnTime(from, to), [from, to])
  return (
    <section>
      <h1>Dispatch on-time report</h1>
      <div className="filters">
        <label>From <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} /></label>
        <label>To <input type="date" value={to} onChange={(e) => setTo(e.target.value)} /></label>
      </div>
      <AsyncView state={state} isEmpty={(r) => r.total === 0} emptyText="No consignments were due in this period.">
        {(report) => (
          <>
            <p className="headline">Network on-time: <strong>{report.onTimePct}%</strong> ({report.onTime} of {report.total})</p>
            <table>
              <thead>
                <tr><th>Depot</th><th>Consignments</th><th>On time</th><th>Late</th><th>On-time %</th></tr>
              </thead>
              <tbody>
                {report.depots.map((d) => (
                  <tr key={d.depotId}>
                    <td>{d.depotName}</td><td>{d.total}</td><td>{d.onTime}</td><td>{d.late}</td><td>{d.onTimePct}%</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </>
        )}
      </AsyncView>
    </section>
  )
}
