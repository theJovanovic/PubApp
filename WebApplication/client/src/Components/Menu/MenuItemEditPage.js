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
    <div className="page-container">

      <div className="title-container">
        <h1>Edit Item Page</h1>
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
                    value={menuItem.name}
                    onChange={handleChange}
                    required
                    />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                    <label>Price:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="number"
                    name="price"
                    value={menuItem.price}
                    onChange={handleChange}
                    min={0}
                    required
                  />
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Category:</label>
                </div>

                <div className="input-wrapper">
                  <select name="category" value={menuItem.category} onChange={handleChange} required>
                    <option value="">Select a category</option>
                    {categories.map((category) => (
                      <option value={category}>{category}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Has allergens:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="checkbox"
                    name="hasAllergens"
                    checked={!!menuItem.hasAllergens}
                    onChange={handleChange}
                  />
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

export default MenuItemEditPage;
