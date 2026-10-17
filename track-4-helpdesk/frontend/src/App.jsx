import { NavLink, Route, Routes } from 'react-router-dom';
import TicketsPage from './pages/TicketsPage.jsx';
import GuidePage from './pages/GuidePage.jsx';

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <h1>IT Helpdesk</h1>
        <nav>
          <NavLink to="/" end>Tickets</NavLink>
          <NavLink to="/guide">Priority guide</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<TicketsPage />} />
          <Route path="/guide" element={<GuidePage />} />
        </Routes>
      </main>
    </div>
  );
}
