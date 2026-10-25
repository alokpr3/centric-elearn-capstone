import { useState } from 'react';
import { reportsApi } from '../api/reports';
import PageState from '../components/PageState';
import { money } from '../components/format';
import { useApi } from '../hooks/useApi';

export default function LossRatioPage() {
  const [from, setFrom] = useState('2025-10-01');
  const [to, setTo] = useState('2026-09-30');
  const { data, loading, error } = useApi(() => reportsApi.lossRatio(from, to), [from, to]);

  return (
    <section>
      <h1>Loss ratio by product</h1>
      <div className="toolbar">
        <label>From <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} /></label>
        <label>To <input type="date" value={to} onChange={(e) => setTo(e.target.value)} /></label>
      </div>
      <PageState loading={loading} error={error} empty={data?.length === 0} emptyText="No data for this period.">
        <table>
          <thead>
            <tr><th>Product</th><th className="num">Written premium</th><th className="num">Paid claims</th><th className="num">Loss ratio</th><th className="num">Settlements</th></tr>
          </thead>
          <tbody>
            {data?.map((r) => (
              <tr key={r.productCode}>
                <td>{r.productName}</td>
                <td className="num">{money(r.writtenPremium)}</td>
                <td className="num">{money(r.paidClaims)}</td>
                <td className="num">{r.lossRatioPct.toFixed(1)}%</td>
                <td className="num">{r.claimCount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </PageState>
    </section>
  );
}
