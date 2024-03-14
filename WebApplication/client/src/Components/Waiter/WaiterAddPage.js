import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

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
        if (response.status === 404) {
          alert("Error: Table doesn't exist")
        }
        throw new Error('Error adding waiter');
      }
      navigate('/waiters');
    } catch (error) {
      console.error('Failed to add waiter:', error);
    }
  };

  return (
    <div>
      <h1>Add Waiter Page</h1>
      <form onSubmit={handleSubmit}>
        <label>
          Name:
          <input 
            type="text" 
            name="name" 
            value={waiter.name} 
            onChange={e => handleChange(e, 0)} 
            required 
          />
        </label>
        <br />
        <button type="submit">Add Waiter</button>
      </form>
    </div>
  );
};

export default WaiterAddPage;