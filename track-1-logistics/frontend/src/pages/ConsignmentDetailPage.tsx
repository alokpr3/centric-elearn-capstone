import { useParams } from 'react-router-dom'
import { getConsignment, listNotes } from '../api/consignments'
import AsyncView from '../components/AsyncView'
import StatusBadge from '../components/StatusBadge'
import { useApi } from '../hooks/useApi'

export default function ConsignmentDetailPage() {
  const id = Number(useParams().id)
  const consignment = useApi(() => getConsignment(id), [id])
  const notes = useApi(() => listNotes(id), [id])
  return (
    <section>
      <AsyncView state={consignment}>
        {(c) => (
          <>
            <h1>{c.reference} <StatusBadge status={c.status} /></h1>
            <dl>
              <dt>Customer</dt><dd>{c.customerName}</dd>
              <dt>Route</dt><dd>{c.originDepot} to {c.destDepot}</dd>
              <dt>Service</dt><dd>{c.serviceLevel}</dd>
              <dt>Parcels / weight</dt><dd>{c.parcels} / {c.weightKg} kg</dd>
              <dt>Promised by</dt><dd>{new Date(c.promisedBy).toLocaleString()}</dd>
              <dt>Driver</dt><dd>{c.driverName ?? 'Not assigned'}</dd>
            </dl>
          </>
        )}
      </AsyncView>
      <h2>Notes</h2>
      <AsyncView state={notes} isEmpty={(n) => n.length === 0} emptyText="No notes yet.">
        {(list) => (
          <ul className="notes">
            {list.map((n) => (
              <li key={n.id}><strong>{n.noteType}</strong> {n.note} <span className="muted">{new Date(n.createdAt).toLocaleString()}</span></li>
            ))}
          </ul>
        )}
      </AsyncView>
    </section>
  )
}
