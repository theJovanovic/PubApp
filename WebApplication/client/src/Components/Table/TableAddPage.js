import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const TableAddPage = () => {
  const navigate = useNavigate();
  const [table, setTable] = useState({
    number: '',
    seats: '',
    status: '',
  });

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
      const endpoint = 'https://localhost:7146/api/Table';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(table),
      });
      if (!response.ok) {
        alert("Error adding table")
        throw new Error('Error adding table');
      }
      navigate('/tables');
    } catch (error) {
      console.error('Failed to add table:', error);
    }
  };

  return (
    <div>
      <h1>Add Table Page</h1>
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
            // Add other statuses as needed
          </select>
        </label>
        <br />
        <button type="submit">Add Table</button>
      </form>
    </div>
  );
};

export default TableAddPage;
