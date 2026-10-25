import { useState } from 'react'
import { searchCustomers } from '../api/customers'
import AsyncView from '../components/AsyncView'
import { useApi } from '../hooks/useApi'

export default function CustomersPage() {
  const [search, setSearch] = useState('')
  const state = useApi(() => searchCustomers(search), [search])
  return (
    <section>
      <h1>Customers</h1>
      <input placeholder="Search by name or account" value={search} onChange={(e) => setSearch(e.target.value)} />
      <AsyncView state={state} isEmpty={(p) => p.items.length === 0} emptyText="No customers match.">
        {(page) => (
          <table>
            <thead>
              <tr><th>Account</th><th>Name</th><th>Contact</th><th>City</th><th>Region</th></tr>
            </thead>
            <tbody>
              {page.items.map((c) => (
                <tr key={c.id}>
                  <td>{c.accountNo}</td><td>{c.name}</td><td>{c.contactName}</td><td>{c.city}</td><td>{c.regionCode}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </AsyncView>
    </section>
  )
}
