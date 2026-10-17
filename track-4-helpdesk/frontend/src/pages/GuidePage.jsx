const GUIDE = [
  { level: 'Urgent', text: 'Work is stopped for many people. Example: email is down.' },
  { level: 'High', text: 'One person cannot work. Example: no VPN access.' },
  { level: 'Medium', text: 'Work is slower but possible. Example: weak battery.' },
  { level: 'Low', text: 'A request that can wait. Example: a new monitor.' },
];

export default function GuidePage() {
  return (
    <section>
      <h2>Which priority should I pick?</h2>
      {GUIDE.map((g) => (
        <div className="card" key={g.level}>
          <span className={`badge ${g.level}`}>{g.level}</span> {g.text}
        </div>
      ))}
    </section>
  );
}
