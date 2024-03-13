import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const WaiterOrderPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    const fetchOrders = async () => {
      const endpoint = `https://localhost:7146/api/Order/overview`;
      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          }
        });
        if (!response.ok) {
          throw new Error('Error fetching orders');
        }
        const data = await response.json();
        setOrders(data);
      } catch (error) {
        console.error('Failed to fetch orders:', error);
      }
    };

    fetchOrders();
  }, []);

  const formatDate = (isoString) => {
    const date = new Date(isoString);
    const formattedDate = new Intl.DateTimeFormat('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    }).format(date);
    return formattedDate;
  }

  return (
    <div>
      <h1>Orders overview</h1>
      <ol>
        {orders.map((order) => (
          <li>
            <h2>Name: {order.name}</h2>
            <h2>Order time: {formatDate(order.orderTime)}</h2>
            <h2>Status: {order.status}</h2>
            <h2>Quantity: {order.quantity}</h2>
            <h2>Table: {order.tableNumber}</h2>
          </li>
        ))}
      </ol>
    </div>
  );
};

export default WaiterOrderPage;
