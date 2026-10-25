import { listHotels } from '../api/hotels'
import StateView from '../components/StateView'
import { useAsync } from '../hooks/useAsync'

export default function HotelsPage() {
  const hotels = useAsync(listHotels, [])
  return (
    <section>
      <h1>Hotels</h1>
      <StateView state={hotels} isEmpty={d => d.length === 0} emptyText="No hotels found.">
        {data => (
          <table>
            <thead>
              <tr><th>Code</th><th>Name</th><th>City</th><th>Region</th><th>Phone</th></tr>
            </thead>
            <tbody>
              {data.map(h => (
                <tr key={h.id}>
                  <td>{h.code}</td><td>{h.name}</td><td>{h.city}</td><td>{h.region}</td><td>{h.phone}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </StateView>
    </section>
  )
}
