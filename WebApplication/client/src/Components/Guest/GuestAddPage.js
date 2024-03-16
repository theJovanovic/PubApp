import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import alertError from '../../alertError';

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
      const endpoint = 'https://localhost:7146/api/Guest';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(guest),
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate('/guests');
    } catch (error) {
      console.error('Failed to add guest:', error);
    }
  };

  return (
    <div className="page-container">

      <div className="title-container">
        <h1>Add Guest Page</h1>
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
                      className="number-input"
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
                    <label>Has discount:</label>
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
                  <option value="">Select table</option>
                  {tables.map((table) => (
                  <option value={table.number}>Table {table.number}</option>
                  ))}
                  </select>
                </div>
              </div>
              
              <div className="form-part-container">
                <div className="helper-container"></div>
                <button className="button-add" type="submit">Add Guest</button>
                <div className="helper-container"></div>
              </div>

            </form>
          </div>

        <div className="helper-container"></div>
      </div>

    </div>
  );
};

export default GuestAddPage;
