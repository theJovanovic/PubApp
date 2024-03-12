import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

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
          throw new Error('Error fetching tables');
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
        throw new Error('Error deleting table');
      }
      setTables(tables.filter(table => table.tableID !== tableID));
    } catch (error) {
      console.error('Failed to delete table:', error);
    }
  };

  return (
    <div>
      <h1>Tables Page</h1>
      <Link to="/tables/add">Add Table</Link>
      <ol>
        {tables.map((table) => (
          <li>
            <h2>Table {table.number}</h2>
            <h3>Seats: {table.seats}</h3>
            <h3>Status: {table.status}</h3>
            <Link to={`/tables/edit/${table.tableID}`}>Edit</Link>
            <Link to={`/tables/info/${table.tableID}`}>Info</Link>
            <a
              style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
              onClick={() => {deleteTable(table.tableID)}}
            >
            Delete
            </a>
          </li>
        ))}
      </ol>
    </div>
  );
};

export default TablesPage;
