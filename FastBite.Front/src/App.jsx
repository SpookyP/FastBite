import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import ListItems from './pages/Items/list-items';
import CreateItem from './pages/Items/create-form';
import EditItem from './pages/Items/edit-form';
// import ShowItem from './pages/Items/details-form';
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/items" element={<ListItems />} />
          <Route path="/items/create" element={<CreateItem />} />
          <Route path="/items/edit" element={<EditItem />} />
          {/* <Route path="/items/show" element={<ShowItem />} />
          <Route path="/items/destroy" element={<DeleteItem />} /> */}
        </Routes>
      </div>
    </Router>
  );
}

export default App;