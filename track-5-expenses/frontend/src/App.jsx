import { NavLink, Route, Routes } from 'react-router-dom';
import ExpensesPage from './pages/ExpensesPage.jsx';
import AddExpensePage from './pages/AddExpensePage.jsx';

export default function App() {
  return (
    <div className="app">
      <header className="topbar">
        <h1>Team Expenses</h1>
        <nav>
          <NavLink to="/" end>Expenses</NavLink>
          <NavLink to="/new">Add expense</NavLink>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<ExpensesPage />} />
          <Route path="/new" element={<AddExpensePage />} />
        </Routes>
      </main>
    </div>
  );
}
