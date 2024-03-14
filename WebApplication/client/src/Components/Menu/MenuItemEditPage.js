import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const MenuItemEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [menuItem, setMenuItem] = useState({});
  const [categories, setCategories] = useState([]);

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
          const message = await alertError(response);
          throw new Error(message);
        }
        const data = await response.json();
        setMenuItem(data);
      } catch (error) {
        console.error('Failed to fetch menu item:', error);
      }
    };

    fetchMenuItem();
  }, []);

  useEffect(() => {
    const fetchMenuCategories = async () => {
      const endpoint = `https://localhost:7146/api/MenuItem/categories`;
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
        setCategories(data);
      } catch (error) {
        console.error('Failed to fetch menu categories:', error);
      }
    };

    fetchMenuCategories();
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
        const message = await alertError(response);
        throw new Error(message);
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
            min={0}
            required
          />
        </label>
        <br />
        <label>
          Category:
          <select name="category" value={menuItem.category} onChange={handleChange} required>
            <option value="">Select a category</option>
            {categories.map((category) => (
              <option value={category}>{category}</option>
            ))}
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
