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

const payOrder = async (orderID, tip) => {
    try {
      const endpoint = `https://localhost:7146/api/Order/pay/${orderID}/${tip}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      const order = guest.orders.find(order => order.orderID === orderID)
      const totalPrice = order.price + tip
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
    <div className="page-container">

      <div className="title-container">
        <h1>Info Guest Page</h1>
      </div>

      <div class="main-container">
        <div className="helper-container"></div>

        <div className="info-container">
          <div className="info-name-container">
            <h4>Name: {guest.name}</h4>
          </div>

          <div className="info-allergies-container">
            <h4>Allergies: {guest.hasAllergies ? 'Yes' : 'No'}</h4>
          </div>

          <div className="info-discount-container">
            <h4>Discount: {guest.hasDiscount ? 'Yes' : 'No'}</h4>
          </div>

          <div className="info-money-container">
            <h4>Money: {guest.money} rsd</h4>
          </div>

          <div className="info-tableNum-container">
            <h4>Table number: <Link to={`/tables/info/${guest.tableID}`}>{guest.tableNumber}</Link></h4>
          </div>

          <div className="info-button-container">
            <div className="helper-container"></div>
            <Link to={`/guests/edit/${guest.guestID}`} className="button-edit">Edit</Link>
            <a className="button-delete" onClick={() => {deleteGuest(guest.guestID)}}>Delete</a>
            <Link to={`/order/add/${guest.guestID}`} className="button-add">Make order</Link>
            <div className="helper-container"></div>
          </div>
        </div>

        <div className="helper-container"></div>
      </div>

      <div className="guest-orders-container">

        <div className="guest-orders-title">
          <h1>Orders:</h1>
        </div>

        <div className="guest-orders-list">
          {guest.orders?.map((order) => (
            <div className="guest-order-container">

              <div className="guest-orderName-container">
                <h4>{order.name} {order.quantity > 1 && `x${order.quantity}`}</h4>
              </div>
              <div className="guest-orderPrice-container">
                <h4>Price: {order.price * order.quantity} din</h4>
              </div>
              <div className="guest-orderStatus-container">
                <h4>{order.status}</h4>
              </div>
              
              {order.status === "Delivered" && (
                <>
                  <h4>Tip (optional):</h4>
                  <input type="number" name="tip" value={tip} onChange={handleChange} min={0} required />
                  <a onClick={() => {payOrder(order.orderID, tip)}} className="button-info">Pay</a>
                </>
              ) || (
                <a id={`cancel_${order.orderID}`} onClick={() => {cancelOrder(order.orderID)}} className="button-delete">Cancel</a>
              )}
            </div>
          ))}
        </div>

      </div>

    </div>
  );
};

export default GuestInfoPage;
