import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const MenuItemEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [menuItem, setMenuItem] = useState({});

  useEffect(() => {
    const fetchMenuItem = async () => {
      const endpoint = `https://localhost:7146/api/MenuItem/${id}`;
      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          throw new Error('Error fetching menu item');
        }
        const data = await response.json();
        setMenuItem(data);
      } catch (error) {
        console.error('Failed to fetch menu item:', error);
      }
    };

    fetchMenuItem();
  }, []);

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
      const endpoint = `https://localhost:7146/api/MenuItem/${id}`;
      const response = await fetch(endpoint, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(menuItem),
      });
      if (!response.ok) {
        if (response.status === 404) {
          alert("Error: Table doesn't exist")
        }
        throw new Error('Error editing menu item');
      }
      navigate('/menu');
    } catch (error) {
      console.error('Failed to edit item:', error);
    }
  };

  return (
    <div>
      <h1>Edit Item Page</h1>
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
        <label>
          Has allergens:
          <input
            type="checkbox"
            name="hasAllergens"
            checked={!!menuItem.hasAllergens}
            onChange={handleChange}
          />
        </label>
        <br />
        <button type="submit">Edit item</button>
      </form>
    </div>
  );
};

export default MenuItemEditPage;
