import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import alertError from '../../alertError';
import "./Waiter.css";

const WaitersPage = () => {
  const [waiters, setWaiters] = useState([])

  useEffect(() => {
    const fetchWaiters = async () => {
      const endpoint = 'https://localhost:7146/api/Waiter';
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
        setWaiters(data);
      } catch (error) {
        console.error('Failed to fetch waiters:', error);
      }
    };

    fetchWaiters();
  }, []);

  const deleteWaiter = async (waiterID) => {
    try {
      const endpoint = `https://localhost:7146/api/Waiter/${waiterID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      setWaiters(waiters.filter(waiter => waiter.waiterID !== waiterID));
    } catch (error) {
      console.error('Failed to delete waiter:', error);
    }
  };

  return (
    <div className="page-container">

      {/* <div className="title-container">
        <h1>Waiters Page</h1>
      </div> */}

      <div className="waiter-add-container">
        <Link to="/waiters/add" className="button-add">Add Waiter</Link>
      </div>

      <div className="waiters-container">
        {waiters.map((waiter) => (
          <div className="waiter-container">
            <div className="waiter-icon-container">
              <img src="waiter.png" alt="Waiter Icon" />
            </div>

            <div className="waiter-info-container">
              <h4>Name: {waiter.name}</h4>
              <h4>Tips: {waiter.tips} rsd</h4>
            </div>

            <div className="waiter-actions-container">
              <Link id={`view_orders_${waiter.waiterID}`} to={`/waiters/orders/${waiter.waiterID}`} className="button-info">View Orders</Link>
              <a id={`delete_${waiter.waiterID}`} onClick={() => {deleteWaiter(waiter.waiterID)}} className="button-delete">Delete</a>
            </div>
          </div>
        ))}
      </div>

    </div>
  );

};

export default WaitersPage;
