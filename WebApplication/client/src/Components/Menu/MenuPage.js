import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

const MenuPage = () => {
    const { id } = useParams();
    const [menuItems, setMenuItems] = useState([]);

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
              throw new Error('Error fetching menu items');
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
          throw new Error('Error deleting item');
        }
        setMenuItems(menuItems.filter(item => item.menuItemID !== menuItemID));
      } catch (error) {
        console.error('Failed to delete item:', error);
      }
    };

    return (
    <div>
        <h1>Menu</h1>
        <Link to="/menu/add">Add menu item</Link>
        <ol>
            {menuItems.map((item) => (
              <>
              <li>
                <h2>{item.name} {item.hasAllergens && "(A)"}</h2>
                <h3>{item.price}din</h3>
                <h3>Category: {item.category}</h3>
                <Link to={`/menu/edit/${item.menuItemID}`}>Edit</Link>
                <a
                  style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
                  onClick={() => {deleteMenuItem(item.menuItemID)}}
                >
                Delete
                </a>
              </li>
              <br />
              </>
            ))}
        </ol>
    </div>
    );
};

export default MenuPage;
