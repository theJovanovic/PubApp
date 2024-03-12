import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const MenuItemAddPage = () => {
  const navigate = useNavigate();
  const [menuItem, setMenuItem] = useState({
    name: '',
    price: '',
    category: '',
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setMenuItem((prevMenuItem) => ({
      ...prevMenuItem,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log('Submitting menuItem:', menuItem);
    try {
      const endpoint = 'https://localhost:7146/api/MenuItem';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(menuItem),
      });
      if (!response.ok) {
        throw new Error('Error adding menu item');
      }
      navigate('/menu');
    } catch (error) {
      console.error('Failed to add menu item:', error);
    }
  };

  return (
    <div>
      <h1>Add MenuItem Page</h1>
      <form onSubmit={handleSubmit}>
        <label>
          Name:
          <input
            type="text"
            name="name"
            value={menuItem.name}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Price:
          <input
            type="number"
            name="price"
            value={menuItem.price}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Category:
          <select name="category" value={menuItem.category} onChange={handleChange} required>
            <option value="">Select a category</option>
            <option value="Italian">Italian</option>
            <option value="Mexican">Mexican</option>
            <option value="Asian">Asian</option>
          </select>
        </label>
        <br />
        <button type="submit">Add menu item</button>
      </form>
    </div>
  );
};

export default MenuItemAddPage;
