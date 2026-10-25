import { useState } from 'react';
import { policiesApi } from '../api/policies';
import PageState from '../components/PageState';
import StatusBadge from '../components/StatusBadge';
import { money } from '../components/format';
import { useApi } from '../hooks/useApi';

export default function PoliciesPage() {
  const [page, setPage] = useState(1);
  const { data, loading, error } = useApi(() => policiesApi.list(page), [page]);

  return (
    <section>
      <h1>Policies</h1>
      <PageState loading={loading} error={error} empty={data?.items.length === 0} emptyText="No policies found.">
        <table>
          <thead>
            <tr><th>Policy</th><th>Policyholder</th><th>Product</th><th>Period</th><th>Status</th><th className="num">Premium</th></tr>
          </thead>
          <tbody>
            {data?.items.map((p) => (
              <tr key={p.id}>
                <td>{p.policyNumber}</td>
                <td>{p.policyholderName}</td>
                <td>{p.productName}</td>
                <td>{p.startDate} to {p.endDate}</td>
                <td><StatusBadge status={p.status} /></td>
                <td className="num">{money(p.annualPremium)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pager">
          <button disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
          <span>Page {page}</span>
          <button disabled={!data || page * data.pageSize >= data.total} onClick={() => setPage(page + 1)}>Next</button>
        </div>
      </PageState>
    </section>
  );
}
