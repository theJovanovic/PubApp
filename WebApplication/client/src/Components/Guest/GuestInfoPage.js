import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

const GuestInfoPage = () => {
    const { id } = useParams();
    const [guest, setGuest] = useState({});

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
              throw new Error('Error fetching guest');
            }
            const data = await response.json();
            setGuest(data);
          } catch (error) {
            console.error('Failed to fetch guest:', error);
          }
        };
    
        fetchGuest();
      }, []);

    return (
    <div>
        <h1>Info Guest Page</h1>
        <h2>Name: {guest.name}</h2>
        <h2>Allergies: {guest.hasAllergies && "Yes" || "No"}</h2>
        <h2>Discount: {guest.hasDiscount && "Yes" || "No"}</h2>
        <h2>Money: {guest.money}</h2>
        <h2>Table number: <Link to={`/tables/info/${guest.tableID}`}>{guest.tableNumber}</Link></h2>
    </div>
    );
};

export default GuestInfoPage;
