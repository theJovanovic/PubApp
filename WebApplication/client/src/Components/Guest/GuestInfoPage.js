import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const mockGuests = [
  {
    guestID: 2121,
    name: "Stefan Jovanovic",
    money: 100,
    hasAllergies: false,
    hasDiscount: true,
    tableNumber: 3,
    tableID: 3,
    orders: [
      {
        orderid: 1,
        ordertime: '2024-03-16T12:00:00Z',
        status: 'Pending',
        name: "Pasta",
        price: 300,
        quantity: 2,
        guestid: 1,
        menuitemid: 101,
        waiterid: 201
      },
      {
        orderid: 2,
        ordertime: '2024-03-16T12:15:00Z',
        status: 'Preparing',
        name: "Fish and Chips",
        price: 280,
        quantity: 1,
        guestid: 2,
        menuitemid: 102,
        waiterid: 202
      },
      {
        orderid: 3,
        ordertime: '2024-03-16T12:30:00Z',
        status: 'Completed',
        name: "Salad",
        price: 250,
        quantity: 3,
        guestid: 3,
        menuitemid: 103,
        waiterid: 203
      },
      {
        orderid: 4,
        ordertime: '2024-03-16T12:45:00Z',
        status: 'Cancelled',
        name: "Pizza",
        price: 350,
        quantity: 1,
        guestid: 4,
        menuitemid: 104,
        waiterid: 204
      },
      {
        orderid: 5,
        ordertime: '2024-03-16T13:00:00Z',
        status: 'Pending',
        name: "Burger",
        price: 400,
        quantity: 4,
        guestid: 5,
        menuitemid: 105,
        waiterid: 205
      },
      {
        orderid: 6,
        ordertime: '2024-03-16T13:15:00Z',
        status: 'Delivered',
        name: "Chicken Wings",
        price: 320,
        quantity: 2,
        guestid: 6,
        menuitemid: 106,
        waiterid: 206
      },
    ]
  },
]

const GuestInfoPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [guest, setGuest] = useState(mockGuests[0]);
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
                  <a onClick={() => {payOrder(order.orderID)}} className="button-info">Pay</a>
                </>
              ) || (
                <a onClick={() => {cancelOrder(order.orderID)}} className="button-delete">Cancel</a>
              )}
            </div>
          ))}
        </div>

      </div>

    </div>
  );
};

export default GuestInfoPage;
