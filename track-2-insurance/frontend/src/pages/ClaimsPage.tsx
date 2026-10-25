import { useState } from 'react';
import { Link } from 'react-router-dom';
import { claimsApi } from '../api/claims';
import PageState from '../components/PageState';
import StatusBadge from '../components/StatusBadge';
import { money } from '../components/format';
import { useApi } from '../hooks/useApi';

const STATUSES = ['', 'New', 'Assessing', 'Approved', 'Declined', 'Settled', 'Closed', 'Reopened'];

export default function ClaimsPage() {
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(1);
  const { data, loading, error } = useApi(() => claimsApi.list(status || undefined, page), [status, page]);

  return (
    <section>
      <h1>Claims</h1>
      <div className="toolbar">
        <label>
          Status{' '}
          <select value={status} onChange={(e) => { setStatus(e.target.value); setPage(1); }}>
            {STATUSES.map((s) => (
              <option key={s} value={s}>{s || 'All'}</option>
            ))}
          </select>
        </label>
      </div>
      <PageState loading={loading} error={error} empty={data?.items.length === 0} emptyText="No claims match this filter.">
        <table>
          <thead>
            <tr><th>Claim</th><th>Policyholder</th><th>Product</th><th>Type</th><th>Handler</th><th>Status</th><th className="num">Estimate</th></tr>
          </thead>
          <tbody>
            {data?.items.map((c) => (
              <tr key={c.id}>
                <td><Link to={`/claims/${c.id}`}>{c.claimNumber}</Link></td>
                <td>{c.policyholderName}</td>
                <td>{c.productName}</td>
                <td>{c.claimType}</td>
                <td>{c.handlerName ?? 'Unassigned'}</td>
                <td><StatusBadge status={c.status} /></td>
                <td className="num">{money(c.estimatedAmount)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pager">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
          <span>Page {page} of {Math.max(1, Math.ceil((data?.total ?? 0) / (data?.pageSize ?? 25)))}</span>
          <button disabled={!data || page * data.pageSize >= data.total} onClick={() => setPage(page + 1)}>Next</button>
        </div>
      </PageState>
    </section>
  );
}
