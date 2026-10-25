import { listDepots } from '../api/depots'
import AsyncView from '../components/AsyncView'
import { useApi } from '../hooks/useApi'

export default function DepotsPage() {
  const state = useApi(() => listDepots(), [])
  return (
    <section>
      <h1>Depots</h1>
      <AsyncView state={state} isEmpty={(d) => d.length === 0} emptyText="No depots found.">
        {(depots) => (
          <table>
            <thead>
              <tr><th>Code</th><th>Name</th><th>Region</th><th>City</th><th>Daily capacity</th></tr>
            </thead>
            <tbody>
              {depots.map((d) => (
                <tr key={d.id}>
                  <td>{d.code}</td><td>{d.name}</td><td>{d.regionCode}</td><td>{d.city}</td><td>{d.dailyCapacity}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </AsyncView>
    </section>
  )
}
