import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import alertError from '../../alertError';

const GuestsPage = () => {
  const [guests, setGuests] = useState([])

  useEffect(() => {
    const fetchGuests = async () => {
      const endpoint = 'https://localhost:7146/api/Guest';
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
        setGuests(data);
      } catch (error) {
        console.error('Failed to fetch guests:', error);
      }
    };

    fetchGuests();
  }, []);

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
      setGuests(guests.filter(guest => guest.guestID !== guestID));
    } catch (error) {
      console.error('Failed to delete guest:', error);
    }
  };

  return (
    <div>
      <h1>Guests Page</h1>
      <Link to="/guests/add">Add Guest</Link>
      <ol>
        {guests.map((guest) => (
          <>
          <li>
            <h2>Name: {guest.name}</h2>
            {/* <h3>Allergies: {guest.hasAllergies && "Yes" || "No"}</h3> */}
            <h3>Table: {guest.tableNumber}</h3>
            <Link to={`/guests/edit/${guest.guestID}`}>Edit</Link>
            <Link to={`/guests/info/${guest.guestID}`}>Info</Link>
            <a
              style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
              onClick={() => {deleteGuest(guest.guestID)}}
            >
            Delete
            </a>
            <Link to={`/order/add/${guest.guestID}`}>Make order</Link>
          </li>
          <br />
          </>
        ))}
      </ol>
    </div>
  );
};

export default GuestsPage;
