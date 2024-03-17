import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import alertError from '../../alertError';
import "./Menu.css";

const mockItems = [
  {
    menuItemID: 1,
    name: "Pasta",
    price: 300,
    category: "Main",
    hasAllergens: false
  },
  {
    menuItemID: 2,
    name: "Fish and Chips",
    price: 280,
    category: "Side",
    hasAllergens: true
  },
  {
    menuItemID: 3,
    name: "Salad",
    price: 250,
    category: "Main",
    hasAllergens: false
  },
  {
    menuItemID: 4,
    name: "Pizza",
    price: 350,
    category: "Main",
    hasAllergens: true
  },
  {
    menuItemID: 5,
    name: "Burger",
    price: 400,
    category: "Main",
    hasAllergens: true
  },
  {
    menuItemID: 6,
    name: "Chicken Wings",
    price: 320,
    category: "Side",
    hasAllergens: false
  }
]

const MenuPage = () => {
    const { id } = useParams();
    const [menuItems, setMenuItems] = useState(mockItems);

    useEffect(() => {
        const fetchMenuItems = async () => {
          const endpoint = `https://localhost:7146/api/MenuItem`;
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
            setMenuItems(data);
          } catch (error) {
            console.error('Failed to fetch menu items:', error);
          }
        };
    
        fetchMenuItems();
      }, []);

    const deleteMenuItem = async (menuItemID) => {
      try {
        const endpoint = `https://localhost:7146/api/MenuItem/${menuItemID}`;
        const response = await fetch(endpoint, {
          method: 'DELETE',
        });
        if (!response.ok) {
          const message = await alertError(response);
          throw new Error(message);
        }
        setMenuItems(menuItems.filter(item => item.menuItemID !== menuItemID));
      } catch (error) {
        console.error('Failed to delete item:', error);
      }
    };

    return (
    <div className="page-container">
        
        <div className="title-container">
          <h1>MENU</h1>
        </div>

        <div className="menu-add-container">
          <Link to="/menu/add" className="button-add">Add Menu Item</Link>
        </div>

        <div className="menu-container">
            {menuItems.map((item) => (
              <div className="item-container">

                <div className="item-icon-container">
                  <img src="menu.png" alt="Menu Item Icon" />
                </div>

                <div className="item-info-container">
                  <h2>{item.name} {item.hasAllergens && "(A)"}</h2>
                  <h3>{item.price} din</h3>
                  <h3>Category: {item.category}</h3>
                </div>

                <div className="item-actions-container">
                  <Link to={`/menu/edit/${item.menuItemID}`} className="button-edit">Edit</Link>
                  <a onClick={() => {deleteMenuItem(item.menuItemID)}} className="button-delete">Delete</a>
                </div>
              </div>
            ))}
        </div>
    </div>
    );
};

export default MenuPage;
