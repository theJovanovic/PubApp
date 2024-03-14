import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const GuestEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [tables, setTables] = useState([])
  const [guest, setGuest] = useState({});
  const [guestCurrentTableNumber, setGuestCurrentTableNumber] = useState();

  useEffect(() => {
    const fetchGuest = async () => {
      const endpoint = `https://localhost:7146/api/Guest/${id}`;
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
        setGuest(data);
        setGuestCurrentTableNumber(data.tableNumber)
      } catch (error) {
        console.error('Failed to fetch guest:', error);
      }
    };

    fetchGuest();
  }, []);

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
      const endpoint = `https://localhost:7146/api/Guest/${id}`;
      const response = await fetch(endpoint, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(guest),
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate(`/guests/info/${id}`);
    } catch (error) {
      console.error('Failed to edit guest:', error);
    }
  };

  return (
    <div>
      <h1>Edit Guest Page</h1>
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
            min={0}
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
            {tables.map((table) => (
              <option value={table.number}>Table {table.number} {table.number === guestCurrentTableNumber && " - Current"}</option>
            ))}
          </select>
        </label>
        <br />
        <button type="submit">Edit Guest</button>
      </form>
    </div>
  );
};

export default GuestEditPage;
