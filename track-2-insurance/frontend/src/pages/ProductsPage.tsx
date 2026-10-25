import { productsApi } from '../api/policies';
import PageState from '../components/PageState';
import { money } from '../components/format';
import { useApi } from '../hooks/useApi';

export default function ProductsPage() {
  const { data, loading, error } = useApi(() => productsApi.list(), []);

  return (
    <section>
      <h1>Products</h1>
      <PageState loading={loading} error={error} empty={data?.length === 0} emptyText="No products defined.">
        <table>
          <thead>
            <tr><th>Code</th><th>Name</th><th>Line</th><th className="num">Base premium</th><th className="num">Default excess</th><th>Active</th></tr>
          </thead>
          <tbody>
            {data?.map((p) => (
              <tr key={p.id}>
                <td>{p.code}</td><td>{p.name}</td><td>{p.line}</td>
                <td className="num">{money(p.basePremium)}</td><td className="num">{money(p.defaultExcess)}</td>
                <td>{p.active ? 'Yes' : 'No'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </PageState>
    </section>
  );
}
