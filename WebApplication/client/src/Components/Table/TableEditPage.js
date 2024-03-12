import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const TableEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [table, setTable] = useState({});

  useEffect(() => {
    const fetchTable = async () => {
      const endpoint = `https://localhost:7146/api/Table/${id}`;
      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          throw new Error('Error fetching table');
        }
        const data = await response.json();
        setTable(data);
      } catch (error) {
        console.error('Failed to fetch table:', error);
      }
    };

    fetchTable();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setTable((prevTable) => ({
      ...prevTable,
      [name]: value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log('Submitting table:', table);
    try {
      const endpoint = `https://localhost:7146/api/Table/${id}`;
      const response = await fetch(endpoint, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(table),
      });
      if (!response.ok) {
        alert("Error editing table")
        throw new Error('Error editing table');
      }
      navigate('/tables');
    } catch (error) {
      console.error('Failed to edit table:', error);
    }
  };

  return (
    <div>
      <h1>Edit Table Page</h1>
      <form onSubmit={handleSubmit}>
        <label>
          Number:
          <input
            type="number"
            name="number"
            value={table.number}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Seats:
          <input
            type="number"
            name="seats"
            value={table.seats}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Status:
          <select name="status" value={table.status} onChange={handleChange} required>
            <option value="">Select a status</option>
            <option value="Available">Available</option>
            <option value="Occupied">Occupied</option>
            <option value="Reserved">Reserved</option>
          </select>
        </label>
        <br />
        <button type="submit">Edit Table</button>
      </form>
    </div>
  );
};

export default TableEditPage;
