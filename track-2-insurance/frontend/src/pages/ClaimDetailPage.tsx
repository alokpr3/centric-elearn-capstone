import { Link, useParams } from 'react-router-dom';
import { claimsApi } from '../api/claims';
import PageState from '../components/PageState';
import StatusBadge from '../components/StatusBadge';
import { money } from '../components/format';
import { useApi } from '../hooks/useApi';

export default function ClaimDetailPage() {
  const id = Number(useParams().id);
  const claim = useApi(() => claimsApi.get(id), [id]);
  const notes = useApi(() => claimsApi.notes(id), [id]);

  return (
    <section>
      <p><Link to="/claims">Back to claims</Link></p>
      <PageState loading={claim.loading} error={claim.error}>
        {claim.data && (
          <>
            <h1>{claim.data.claimNumber} <StatusBadge status={claim.data.status} /></h1>
            <dl className="facts">
              <dt>Type</dt><dd>{claim.data.claimType}</dd>
              <dt>Loss date</dt><dd>{claim.data.lossDate}</dd>
              <dt>Estimate</dt><dd>{money(claim.data.estimatedAmount)}</dd>
              <dt>Approved</dt><dd>{money(claim.data.approvedAmount)}</dd>
              <dt>Paid</dt><dd>{money(claim.data.paidAmount)}</dd>
              <dt>Excess</dt><dd>{money(claim.data.excess)}</dd>
              <dt>Reopened</dt><dd>{claim.data.reopenedCount} time(s)</dd>
            </dl>
            <p>{claim.data.description}</p>
          </>
        )}
      </PageState>
      <h2>Notes</h2>
      <PageState loading={notes.loading} error={notes.error} empty={notes.data?.length === 0} emptyText="No notes on this claim yet.">
        <ul className="notes">
          {notes.data?.map((n) => (
            <li key={n.id}><strong>{n.noteType}</strong> <small>{n.createdAt}</small><br />{n.body}</li>
          ))}
        </ul>
      </PageState>
    </section>
  );
}
