import { NavLink, Route, Routes } from 'react-router-dom'
import HotelsPage from './pages/HotelsPage'
import BookingsPage from './pages/BookingsPage'
import GuestsPage from './pages/GuestsPage'
import OccupancyPage from './pages/OccupancyPage'

export default function App() {
  return (
    <div className="shell">
      <header className="topbar">
        <span className="brand">Larchmont Hotels</span>
        <nav>
          <NavLink to="/">Hotels</NavLink>
          <NavLink to="/bookings">Bookings</NavLink>
          <NavLink to="/guests">Guests</NavLink>
          <NavLink to="/reports/occupancy">Occupancy</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<HotelsPage />} />
          <Route path="/bookings" element={<BookingsPage />} />
          <Route path="/guests" element={<GuestsPage />} />
          <Route path="/reports/occupancy" element={<OccupancyPage />} />
        </Routes>
      </main>
    </div>
  )
}
