import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const GuestInfoPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [guest, setGuest] = useState({});
  const [tip, setTip] = useState(0)

  // Fetch guest
  useEffect(() => {
      const fetchGuest = async () => {
        const endpoint = `https://localhost:7146/api/Guest/info/${id}`;
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

  const cancelOrder = async (orderID) => {
    try {
      const endpoint = `https://localhost:7146/api/Order/${orderID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      setGuest(prevGuest => ({
        ...prevGuest,
        orders: prevGuest.orders.filter(order => order.orderID !== orderID)
      }));
    } catch (error) {
      console.error('Failed to cancel order:', error);
    }
  };

  const payOrder = async (orderID) => {
    try {
      const endpoint = `https://localhost:7146/api/Order/pay/${orderID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
        body: JSON.stringify(tip),
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      const order = guest.orders.find(order => order.orderID === orderID)
      const totalPrice = order.price * order.quantity
      setGuest(prevGuest => ({
        ...prevGuest,
        money: prevGuest.money - totalPrice,
        orders: prevGuest.orders.filter(order => order.orderID !== orderID)
      }));
    } catch (error) {
      console.error('Failed to pay order:', error);
    }
  }

  const handleChange = (e) => {
    const { _, value } = e.target;
    setTip(value);
  };

  const deleteGuest = async (guestID) => {
    try {
      const endpoint = `https://localhost:7146/api/Guest/${guestID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate('/guests')
    } catch (error) {
      console.error('Failed to delete guest:', error);
    }
  };

  return (
  <div>
      <h1>Info Guest Page</h1>
      <h2>Name: {guest.name}</h2>
      <h2>Allergies: {guest.hasAllergies && "Yes" || "No"}</h2>
      <h2>Discount: {guest.hasDiscount && "Yes" || "No"}</h2>
      <h2>Money: {guest.money}</h2>
      <h2>Table number: <Link to={`/tables/info/${guest.tableID}`}>{guest.tableNumber}</Link></h2>
      <Link to={`/guests/edit/${guest.guestID}`}>Edit</Link>
      <a
        style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
        onClick={() => {deleteGuest(guest.guestID)}}
      >
      Delete
      </a>
      <Link to={`/order/add/${guest.guestID}`}>Make order</Link>
      <h2>Orders:</h2>
      <ol>
        {guest.orders?.map((order) => (
          <>
          <li>
            <h3>{order.name} {order.quantity > 1 && `x${order.quantity}`}</h3>
            <h3>{order.price * order.quantity}</h3>
            <h3>{order.status}</h3>
            {order.status === "Delivered" && (
              <>
              <label>
                Tip (optional):
                <input
                  type="number"
                  name="tip"
                  value={tip}
                  onChange={handleChange}
                  min={0}
                  required
                />
              </label>
              <br />
              <a
                style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
                onClick={() => {payOrder(order.orderID)}}
              >
              Pay
              </a>
              </>
            ) || (
              <a
                style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
                onClick={() => {cancelOrder(order.orderID)}}
              >
              Cancel
              </a>
            )}
          </li>
          <br />
          </>
        ))}
      </ol>
  </div>
  );
};

export default GuestInfoPage;
