import { useState } from 'react';
import { policyholdersApi } from '../api/policies';
import PageState from '../components/PageState';
import { useApi } from '../hooks/useApi';

export default function PolicyholdersPage() {
  const [search, setSearch] = useState('');
  const { data, loading, error } = useApi(() => policyholdersApi.list(search), [search]);

  return (
    <section>
      <h1>Policyholders</h1>
      <div className="toolbar">
        <input placeholder="Search name, email or postcode" value={search} onChange={(e) => setSearch(e.target.value)} />
      </div>
      <PageState loading={loading} error={error} empty={data?.items.length === 0} emptyText="No policyholders match your search.">
        <table>
          <thead>
            <tr><th>Name</th><th>Email</th><th>Phone</th><th>City</th><th>Postcode</th></tr>
          </thead>
          <tbody>
            {data?.items.map((h) => (
              <tr key={h.id}>
                <td>{h.fullName}</td><td>{h.email}</td><td>{h.phone}</td><td>{h.city}</td><td>{h.postcode}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </PageState>
    </section>
  );
}
