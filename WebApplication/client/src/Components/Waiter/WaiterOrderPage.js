import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import alertError from '../../alertError';

const WaiterOrderPage = () => {
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
          const message = await alertError(response);
          throw new Error(message);
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

  const deliverOrder = async (orderID) => {
    try {
      const endpoint = `https://localhost:7146/api/Order/${orderID}/Waiter/${id}`;
      const response = await fetch(endpoint, {
        method: 'PUT',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      setOrders(orders.filter(order => order.orderID !== orderID));
    } catch (error) {
      console.error('Failed to deliver order:', error);
    }
  }

  return (
    <div className="page-container">

      <div className="title-container">
        <h1>Orders Overview</h1>
      </div>

      <div className="waiters-container">
        {orders.map((order) => (
          <div className="waiter-container">

            <div className="waiter-info-container">
              <h4>Name: {order.name}</h4>
              <h4>Order time: {formatDate(order.orderTime)}</h4>
              <h4>Status: {order.status}</h4>
              <h4>Quantity: {order.quantity}</h4>
              <h4>Table: {order.tableNumber}</h4>
            </div>

            <div className="waiter-actions-container">
              {order.status === "Completed" && 
                <a id={`deliver_${order.orderID}`} onClick={() => {deliverOrder(order.orderID)}} className="button-info">Deliver</a>
              }
            </div>
          </div>
        ))}
      </div>

    </div>
  );

};

export default WaiterOrderPage;
