import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import AdminDashboard from './pages/Admin/AdminDashboard';
import CreateItem from './pages/Admin/CreateItem'; // <-- Importar a nova vista
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/admin" element={<AdminDashboard />} />
          <Route path="/admin/menus/create" element={<CreateItem />} /> {/* <-- Nova Rota */}
        </Routes>
      </div>
    </Router>
  );
}

export default App;