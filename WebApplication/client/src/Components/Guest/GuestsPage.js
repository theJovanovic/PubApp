import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import alertError from '../../alertError';
import './Guest.css';

const GuestsPage = () => {
  const [guests, setGuests] = useState([
    {
      guestid: 1,
      name: "Stefan Jovanovic",
      money: 100,
      hasAllergies: false,
      hasDiscount: false,
      tableNumber: 3
    },
    {
      guestid: 2,
      name: "Ena Separovic",
      money: 80,
      hasAllergies: true,
      hasDiscount: true,
      tableNumber: 3
    },
    {
      guestid: 3,
      name: "Ivan Horvat",
      money: 120,
      hasAllergies: false,
      hasDiscount: false,
      tableNumber: 3
    },
    {
      guestid: 4,
      name: "Luka Modric",
      money: 150,
      hasAllergies: true,
      hasDiscount: true,
      tableNumber: 3
    },
    {
      guestid: 5,
      name: "Marija Novak",
      money: 200,
      hasAllergies: false,
      hasDiscount: false,
      tableNumber: 3
    },
    {
      guestid: 6,
      name: "Ana Kovačić",
      money: 50,
      hasAllergies: true,
      hasDiscount: true,
      tableNumber: 3
    }
  ])

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
              <Link to={`/guests/edit/${guest.guestID}`} className="button-edit">Edit</Link>
              <Link to={`/guests/info/${guest.guestID}`} className="button-info">Info</Link>
              <a className="button-delete" onClick={() => {deleteGuest(guest.guestID)}}>Delete</a>
              <Link to={`/order/add/${guest.guestID}`} className="button-add">Make order</Link>
            </div>
          </div>
        ))}
      </div>

    </div>
  );
};

export default GuestsPage;
