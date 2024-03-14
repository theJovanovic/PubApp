import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const GuestAddPage = () => {
  const navigate = useNavigate();
  const [tables, setTables] = useState([])
  const [guest, setGuest] = useState({
    name: '',
    money: '',
    hasAllergies: false,
    hasDiscount: false,
    tableNumber: '',
  });

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

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setGuest((prevGuest) => ({
      ...prevGuest,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log('Submitting guest:', guest);
    try {
      const endpoint = 'https://localhost:7146/api/Guest';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(guest),
      });
      if (!response.ok) {
        if (response.status === 404) {
          alert("Error: Table doesn't exist")
        }
        else if (response.status == 409) {
          alert("Error: Table is full")
        }
        throw new Error('Error adding guest');
      }
      navigate('/guests');
    } catch (error) {
      console.error('Failed to add guest:', error);
    }
  };

  return (
    <div>
      <h1>Add Guest Page</h1>
      <form onSubmit={handleSubmit}>
        <label>
          Name:
          <input
            type="text"
            name="name"
            value={guest.name}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Money:
          <input
            type="number"
            name="money"
            value={guest.money}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Has allergies:
          <input
            type="checkbox"
            name="hasAllergies"
            checked={!!guest.hasAllergies}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Has discount:
          <input
            type="checkbox"
            name="hasDiscount"
            checked={!!guest.hasDiscount}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Select table:
          <select name="tableNumber" value={guest.tableNumber} onChange={handleChange} required>
            <option value="">Select table</option>
            {tables.map((table) => (
              <option value={table.number}>Table {table.number}</option>
            ))}
          </select>
        </label>
        <br />
        <button type="submit">Add Guest</button>
      </form>
    </div>
  );
};

export default GuestAddPage;
