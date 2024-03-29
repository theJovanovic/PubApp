import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import alertError from '../../alertError';
import './Table.css';

const TablesPage = () => {
  const [tables, setTables] = useState([])

  useEffect(() => {
    const fetchTables = async () => {
      const endpoint = 'https://localhost:7146/api/Table';
      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          const message = await alertError(response);
          throw new Error(message);
        }
        const data = await response.json();
        setTables(data);
      } catch (error) {
        console.error('Failed to fetch tables:', error);
      }
    };

    fetchTables();
  }, []);

  const deleteTable = async (tableID) => {
    try {
      const endpoint = `https://localhost:7146/api/Table/${tableID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      setTables(tables.filter(table => table.tableID !== tableID));
    } catch (error) {
      console.error('Failed to delete table:', error);
    }
  };

  return (
    <div className="page-container">
      <Link to="/tables/add" className="button-add">Add Table</Link>

      <div className="tables-container">
        {tables.map((table) => (
          <div key={table.tableID} className="table-wrapper">
            <div className="table-circle">
              <p>Table {table.number}</p>
              <p>Seats: {table.seats}</p>
              <p>Status: {table.status}</p>
            </div>
            <div className="table-actions">
              <Link id={`edit_${table.tableID}`} to={`/tables/edit/${table.tableID}`} className="button-edit">Edit</Link>
              <Link id={`info_${table.tableID}`} to={`/tables/info/${table.tableID}`} className="button-info">Info</Link>
              <button id={`delete_${table.tableID}`} className="button-delete" onClick={() => deleteTable(table.tableID)}>Delete</button>
            </div>
          </div>
        ))}
      </div>

    </div>
  );
};

export default TablesPage;
