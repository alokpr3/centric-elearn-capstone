import { NavLink, Route, Routes } from 'react-router-dom'
import ConsignmentsPage from './pages/ConsignmentsPage'
import ConsignmentDetailPage from './pages/ConsignmentDetailPage'
import CustomersPage from './pages/CustomersPage'
import DepotsPage from './pages/DepotsPage'
import DispatchReportPage from './pages/DispatchReportPage'
import ImportPage from './pages/ImportPage'

export default function App() {
  return (
    <div className="shell">
      <header className="topbar">
        <span className="brand">Northgate Freight</span>
        <nav>
          <NavLink to="/">Consignments</NavLink>
          <NavLink to="/depots">Depots</NavLink>
          <NavLink to="/customers">Customers</NavLink>
          <NavLink to="/reports/dispatch">Dispatch report</NavLink>
          <NavLink to="/import">Import</NavLink>
        </nav>
      </header>
      <main className="content">
        <Routes>
          <Route path="/" element={<ConsignmentsPage />} />
          <Route path="/consignments/:id" element={<ConsignmentDetailPage />} />
          <Route path="/depots" element={<DepotsPage />} />
          <Route path="/customers" element={<CustomersPage />} />
          <Route path="/reports/dispatch" element={<DispatchReportPage />} />
          <Route path="/import" element={<ImportPage />} />
        </Routes>
      </main>
    </div>
  )
}
