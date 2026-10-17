import { useEffect, useState } from 'react';
import { getDoctors } from '../api.js';

export default function DoctorsPage() {
  const [doctors, setDoctors] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    getDoctors().then(setDoctors).catch((e) => setError(e.message));
  }, []);

  return (
    <section>
      <h2>Our doctors</h2>
      {error && <div className="message error">{error}</div>}
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Speciality</th>
          </tr>
        </thead>
        <tbody>
          {doctors.map((d) => (
            <tr key={d.id}>
              <td>{d.name}</td>
              <td>{d.specialty}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
