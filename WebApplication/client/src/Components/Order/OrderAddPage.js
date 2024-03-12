import React, { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const OrderAddPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [order, setOrder] = useState({
    quantity: '',
    name: '',
    price: false,
    hasDiscount: false,
    tableNumber: '',
  });

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setOrder((prevOrder) => ({
      ...prevOrder,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };
  
  const handleSubmit = async (e) => {
    e.preventDefault();
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
      navigate('/orders');
    } catch (error) {
      console.error('Failed to add order:', error);
    }
  };

  return (
    <div>
      <h1>Add Order Page</h1>
      <form onSubmit={handleSubmit}>
        <label>
          Name:
          <input
            type="text"
            name="name"
            value={order.name}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Money:
          <input
            type="number"
            name="money"
            value={order.money}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Has allergies:
          <input
            type="checkbox"
            name="hasAllergies"
            checked={!!order.hasAllergies}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Has discount:
          <input
            type="checkbox"
            name="hasDiscount"
            checked={!!order.hasDiscount}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Table number:
          <input
            type="number"
            name="tableNumber"
            value={order.tableNumber}
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
