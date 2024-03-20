import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import alertError from '../../alertError';
import './Table.css';

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
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate('/tables');
    } catch (error) {
      console.error('Failed to add table:', error);
    }
  };

  return (
    <div className="page-container">

      <div className="title-container">
        <h1>Add Table Page</h1>
      </div>

      <div class="main-container">
        <div className="helper-container"></div>

          <div className="form-container">
            <form onSubmit={handleSubmit}>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Number:</label>
                </div>

                <div className="input-wrapper">
                  <input
                      className="number-input"
                      type="number"
                      name="number"
                      value={table.number}
                      onChange={handleChange}
                      min={1}
                      required
                    />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                    <label>Seats:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    className='seats-input'
                    type="number"
                    name="seats"
                    value={table.seats}
                    onChange={handleChange}
                    min={1}
                    required
                  />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Status:</label>
                </div>

                <div className="input-wrapper">
                  <select name="status" value={table.status} onChange={handleChange} required>
                    <option value="">Select status</option>
                    <option value="Available">Available</option>
                    <option value="Occupied">Occupied</option>
                    <option value="Full">Full</option>
                  </select>
                </div>
              </div>
              
              <div className="form-part-container">
                <div className="helper-container"></div>
                <button className="button-add" type="submit">Add Table</button>
                <div className="helper-container"></div>
              </div>

            </form>
          </div>

        <div className="helper-container"></div>
      </div>

    </div>
  );
};

export default TableAddPage;
