import './App.css';
import React from "react";
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import NavigationHeader from './Components/NavigationHeader';
import TablesPage from './Components/Table/TablesPage';
import GuestsPage from './Components/Guest/GuestsPage';
import TableInfoPage from './Components/Table/TableInfoPage';
import TableEditPage from './Components/Table/TableEditPage';
import TableAddPage from './Components/Table/TableAddPage';
import GuestAddPage from './Components/Guest/GuestAddPage';
import GuestEditPage from './Components/Guest/GuestEditPage';
import GuestInfoPage from './Components/Guest/GuestInfoPage';
import OrderAddPage from './Components/Order/OrderAddPage';
import MenuPage from './Components/Menu/MenuPage';
import MenuItemAddPage from './Components/Menu/MenuItemAddPage';

function App() {
  return (
    <BrowserRouter>
      <NavigationHeader />
      <Routes>
        <Route path="/" element={<Navigate replace to="/tables" />} />
        <Route path="/tables" element={<TablesPage />} />
        <Route path="/tables/add" element={<TableAddPage />} />
        <Route path="/tables/edit/:id" element={<TableEditPage />} />
        <Route path="/tables/info/:id" element={<TableInfoPage />} />
        <Route path="/guests" element={<GuestsPage />} />
        <Route path="/guests/add" element={<GuestAddPage />} />
        <Route path="/guests/edit/:id" element={<GuestEditPage />} />
        <Route path="/guests/info/:id" element={<GuestInfoPage />} />
        <Route path="/order/add/:id" element={<OrderAddPage />} />
        <Route path="/menu" element={<MenuPage />} />
        <Route path="/menu/add" element={<MenuItemAddPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
