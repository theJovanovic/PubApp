import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import alertError from '../../alertError';

const WaiterAddPage = () => {
  const navigate = useNavigate();
  const [waiter, setWaiter] = useState({
    name: '',
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setWaiter((prevWaiter) => ({
      ...prevWaiter,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log('Submitting waiter:', waiter);
    try {
      const endpoint = 'https://localhost:7146/api/Waiter';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(waiter),
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate('/waiters');
    } catch (error) {
      console.error('Failed to add waiter:', error);
    }
  };

  return (
    <div className="page-container">

      <div className="title-container">
        <h1>Add Waiter Page</h1>
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
                    value={waiter.name} 
                    onChange={e => handleChange(e, 0)} 
                    required
                    />
                </div>
              </div>
              
              <div className="form-part-container">
                <div className="helper-container"></div>
                <button className="button-add" type="submit">Add Waiter</button>
                <div className="helper-container"></div>
              </div>

            </form>
          </div>

        <div className="helper-container"></div>
      </div>

    </div>
  );
};

export default WaiterAddPage;