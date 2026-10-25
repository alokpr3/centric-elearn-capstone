import { useState } from 'react'
import { Link } from 'react-router-dom'
import { listStatuses, searchConsignments } from '../api/consignments'
import AsyncView from '../components/AsyncView'
import StatusBadge from '../components/StatusBadge'
import { useApi } from '../hooks/useApi'

export default function ConsignmentsPage() {
  const [status, setStatus] = useState('')
  const [search, setSearch] = useState('')
  const statuses = useApi(() => listStatuses(), [])
  const state = useApi(() => searchConsignments(status, search), [status, search])
  return (
    <section>
      <h1>Consignments</h1>
      <div className="filters">
        <input placeholder="Reference" value={search} onChange={(e) => setSearch(e.target.value)} />
        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          <option value="">All statuses</option>
          {(statuses.data ?? []).map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>
      <AsyncView state={state} isEmpty={(p) => p.items.length === 0} emptyText="No consignments match.">
        {(page) => (
          <>
            <p className="muted">{page.total} consignments</p>
            <table>
              <thead>
                <tr><th>Reference</th><th>Customer</th><th>From</th><th>To</th><th>Service</th><th>Status</th><th>Promised by</th></tr>
              </thead>
              <tbody>
                {page.items.map((c) => (
                  <tr key={c.id}>
                    <td><Link to={`/consignments/${c.id}`}>{c.reference}</Link></td>
                    <td>{c.customerName}</td><td>{c.originDepot}</td><td>{c.destDepot}</td><td>{c.serviceLevel}</td>
                    <td><StatusBadge status={c.status} /></td>
                    <td>{new Date(c.promisedBy).toLocaleString()}</td>
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
