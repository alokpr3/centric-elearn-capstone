import { NavLink, Route, Routes } from 'react-router-dom';
import MenuPage from './pages/MenuPage.jsx';
import OrdersPage from './pages/OrdersPage.jsx';

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <h1>Office Canteen</h1>
        <nav>
          <NavLink to="/" end>Menu</NavLink>
          <NavLink to="/new">Orders</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<MenuPage />} />
          <Route path="/new" element={<OrdersPage />} />
        </Routes>
      </main>
    </div>
  );
}
