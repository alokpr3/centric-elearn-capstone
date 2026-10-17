import { NavLink, Route, Routes } from 'react-router-dom';
import DoctorsPage from './pages/DoctorsPage.jsx';
import BookPage from './pages/BookPage.jsx';

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <h1>City Clinic</h1>
        <nav>
          <NavLink to="/" end>Doctors</NavLink>
          <NavLink to="/new">Book visit</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<DoctorsPage />} />
          <Route path="/new" element={<BookPage />} />
        </Routes>
      </main>
    </div>
  );
}
