import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

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
          const message = await alertError(response);
          throw new Error(message);
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
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate(`/guests/info/${id}`);
    } catch (error) {
      console.error('Failed to add order:', error);
    }
  };

  return (
    <div className="page-container">

      <div className="title-container">
        <h1>Add Order Page</h1>
      </div>

      <div className="order-title-container">
        <h3>Available: {guest.money} rsd
          {guest.hasDiscount && " (15% discount applied)"}
        </h3>
      </div>

      <div class="main-container">
        <div className="helper-container"></div>

          <div className="form-container">
            <form onSubmit={handleSubmit}>

              <div className="form-part-container">
                <div className="label-wrapper">
                  <label>Select dish:</label>
                </div>

                <div className="input-wrapper">
                  <select name="menuItemID" value={order.menuItemID} onChange={handleChange} required>
                    <option value="">Select a item</option>
                    {menuItems.map((item) => (
                      <option value={item.menuItemID}>{item.name} ({item.category}) - {item.price} rsd</option>
                    ))}
                  </select>

                  {hasAllergens && (
                    <label>
                      *Has allergens
                    </label>
                  )}
                  
                </div>
              </div>

              <div className="form-part-container">
                <div className="label-wrapper">
                    <label>Quantity:</label>
                </div>

                <div className="input-wrapper">
                  <input
                    type="number"
                    name="quantity"
                    value={order.quantity}
                    onChange={handleChange}
                    min={1}
                    required
                  />
                </div>
              </div>
              
              <div className="form-part-container">
                <div className="helper-container"></div>
                <button className="button-add" type="submit">Add Order</button>
                <div className="helper-container"></div>
              </div>

            </form>
          </div>

        <div className="helper-container"></div>
      </div>

    </div>
  );
};

export default OrderAddPage;
