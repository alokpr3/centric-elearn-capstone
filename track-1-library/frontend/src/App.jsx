import { NavLink, Route, Routes } from 'react-router-dom';
import BooksPage from './pages/BooksPage.jsx';
import BorrowPage from './pages/BorrowPage.jsx';

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <h1>Library Desk</h1>
        <nav>
          <NavLink to="/" end>Books</NavLink>
          <NavLink to="/new">Borrow</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<BooksPage />} />
          <Route path="/new" element={<BorrowPage />} />
        </Routes>
      </main>
    </div>
  );
}
