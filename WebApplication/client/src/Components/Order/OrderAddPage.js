import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const OrderAddPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [guest, setGuest] = useState({});
  const [menuItems, setMenuItems] = useState([]);
  const [order, setOrder] = useState({
    guestID: '',
    menuItemID: '',
    quantity: '1',
  });
  const [hasAllergens, setHasAllergens] = useState(false)

  useEffect(() => {
    const fetchGuest = async () => {
      const endpoint = `https://localhost:7146/api/Guest/${id}`;
      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          throw new Error('Error fetching guest');
        }
        const data = await response.json();
        setGuest(data);
      } catch (error) {
        console.error('Failed to fetch guest:', error);
      }
    };

    fetchGuest();
  }, []);

  // Fetch menu
  useEffect(() => {
    const fetchMenuItems = async () => {
      const endpoint = `https://localhost:7146/api/MenuItem/order/${id}`;
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

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setOrder((prevOrder) => ({
      ...prevOrder,
      [name]: type === 'checkbox' ? checked : value,
    }));
    if (name == 'menuItemID') {
      if (value === "") {
        setHasAllergens(false)
      }
      else {
        const menuItemID = parseInt(value, 10);
        const item = menuItems.find((item) => item.menuItemID === menuItemID);
        setHasAllergens(item.hasAllergens)
      }
    }
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
    order.guestID = id;
    console.log('Submitting order:', order);
    try {
      const endpoint = 'https://localhost:7146/api/Order';
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(order),
      });
      if (!response.ok) {
        if (response.status === 404) {
          alert("Error: Table doesn't exist")
        }
        throw new Error('Error adding order');
      }
      navigate(`/guests/info/${id}`);
    } catch (error) {
      console.error('Failed to add order:', error);
    }
  };

  return (
    <div>
      <h1>Add Order Page</h1>
      <h3>Available: {guest.money}din
      {guest.hasDiscount && " (15% discount applied)"}
      </h3>
      <form onSubmit={handleSubmit}>
        <label>
          Select dish:
          <select name="menuItemID" value={order.menuItemID} onChange={handleChange} required>
            <option value="">Select a item</option>
            {menuItems.map((item) => (
              <option value={item.menuItemID}>{item.name} ({item.category}) - {item.price}din</option>
            ))}
          </select>
        </label>
        {hasAllergens && (
          <>
          <br />
          <label>
            *Has allergens
          </label>
          </>
        )}
        <br />
        <label>
          Quantity:
          <input
            type="number"
            name="quantity"
            value={order.quantity}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <button type="submit">Add Order</button>
      </form>
    </div>
  );
};

export default OrderAddPage;
