import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

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
          throw new Error('Error fetching waiters');
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
        throw new Error('Error deleting waiter');
      }
      setWaiters(waiters.filter(waiter => waiter.waiterID !== waiterID));
    } catch (error) {
      console.error('Failed to delete waiter:', error);
    }
  };

  return (
    <div>
      <h1>Waiters Page</h1>
      <Link to="/waiters/add">Add Waiter</Link>
      <ol>
        {waiters.map((waiter) => (
          <>
          <li>
            <h2>{waiter.name}</h2>
            <h3>Tips: {waiter.tips}din</h3>
            <Link to={`/waiters/orders/${waiter.waiterID}`}>View orders</Link>
            <a
              style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
              onClick={() => {deleteWaiter(waiter.waiterID)}}
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

export default WaitersPage;
