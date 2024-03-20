import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import alertError from '../../alertError';
import './Guest.css';

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
    <div className="page-container">

      {/* <div className="title-container">
        <h1>Guests Page</h1>
      </div> */}

      <div className="guest-add-container">
        <Link to="/guests/add" className="button-add">Add Guest</Link>
      </div>

      <div className="guests-container">
        {guests.map((guest) => (
          <div className="guest-container">
            <div className="guest-icon-container">
              <img src="user.png" alt="Guest Icon" />
            </div>

            <div className="guest-info-container">
              <h4>Name: {guest.name}</h4>
              <h4>Allergies: {guest.hasAllergies && "Yes" || "No"}</h4>
              <h4>Table: {guest.tableNumber}</h4>
            </div>

            <div className="guest-actions-container">
              <Link id={`edit_${guest.guestID}`} to={`/guests/edit/${guest.guestID}`} className="button-edit">Edit</Link>
              <Link id={`info_${guest.guestID}`} to={`/guests/info/${guest.guestID}`} className="button-info">Info</Link>
              <a id={`delete_${guest.guestID}`} className="button-delete" onClick={() => {deleteGuest(guest.guestID)}}>Delete</a>
              <Link id={`make_order_${guest.guestID}`} to={`/order/add/${guest.guestID}`} className="button-add">Make order</Link>
            </div>
          </div>
        ))}
      </div>

    </div>
  );
};

export default GuestsPage;
