import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const mockTables = [
  {
    tableID: "1",
    number: 1,
    seats: 4,
    status: "Available",
  },
  {
    tableID: "2",
    number: 2,
    seats: 2,
    status: "Occupied",
  },
  {
    tableID: "3",
    number: 3,
    seats: 6,
    status: "Full",
  },
  {
    tableID: "4",
    number: 4,
    seats: 4,
    status: "Available",
  }
];

const GuestEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [tables, setTables] = useState(mockTables)
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
    <div className="page-container">

      <div className="title-container">
        <h1>Edit Guest Page</h1>
      </div>

      <div class="main-container">
        <div className="helper-container"></div>

          <div className="form-container">
            <form onSubmit={handleSubmit}>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Name:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="text"
                    name="name"
                    value={guest.name}
                    onChange={handleChange}
                    required
                    />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                    <label>Money:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="number"
                    name="money"
                    value={guest.money}
                    onChange={handleChange}
                    min={0}
                    required
                  />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Has allergies:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="checkbox"
                    name="hasAllergies"
                    checked={!!guest.hasAllergies}
                    onChange={handleChange}
                  />
                </div>
              </div>
              
              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Has allergies:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="checkbox"
                    name="hasDiscount"
                    checked={!!guest.hasDiscount}
                    onChange={handleChange}
                  />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Select table:</label>
                </div>

                <div className="input-wrapper">
                <select name="tableNumber" value={guest.tableNumber} onChange={handleChange} required>
                  {tables.map((table) => (
                    <option value={table.number}>Table {table.number} {table.number === guestCurrentTableNumber && " - Current"}</option>
                  ))}
                </select>
                </div>
              </div>

              <div className="form-part-container">
                <div className="helper-container"></div>
                <button className="button-edit" type="submit">Edit Guest</button>
                <div className="helper-container"></div>
              </div>

            </form>
          </div>

        <div className="helper-container"></div>
      </div>

    </div>
  );
};

export default GuestEditPage;
