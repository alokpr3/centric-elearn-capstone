import { useEffect, useState } from 'react';
import { bookAppointment, getDoctors } from '../api.js';

export default function BookPage() {
  const [doctors, setDoctors] = useState([]);
  const [doctorId, setDoctorId] = useState('');
  const [patientName, setPatientName] = useState('');
  const [start, setStart] = useState('');
  const [message, setMessage] = useState(null);

  useEffect(() => {
    getDoctors().then((list) => {
      setDoctors(list);
      if (list.length > 0) setDoctorId(String(list[0].id));
    });
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      const appt = await bookAppointment({
        doctorId: Number(doctorId),
        patientName,
        start,
        minutes: 30,
      });
      const when = new Date(appt.start).toLocaleString();
      setMessage({ text: `Booked for ${when}.`, error: false });
      setPatientName('');
      setStart('');
    } catch (err) {
      setMessage({ text: err.message, error: true });
    }
  }

  return (
    <section>
      <h2>Book a visit</h2>
      {message && (
        <div className={message.error ? 'message error' : 'message'}>{message.text}</div>
      )}
      <form onSubmit={handleSubmit}>
        <label>
          Doctor
          <select value={doctorId} onChange={(e) => setDoctorId(e.target.value)}>
            {doctors.map((d) => (
              <option key={d.id} value={d.id}>
                {d.name} ({d.specialty})
              </option>
            ))}
          </select>
        </label>
        <label>
          Patient name
          <input value={patientName} onChange={(e) => setPatientName(e.target.value)} />
        </label>
        <label>
          Date and time (30 minutes)
          <input type="datetime-local" value={start} onChange={(e) => setStart(e.target.value)} />
        </label>
        <button type="submit">Book</button>
      </form>
    </section>
  );
}
