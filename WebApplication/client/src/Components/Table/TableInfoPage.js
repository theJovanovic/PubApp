import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';

const TableInfoPage = () => {
    const { id } = useParams();
    const [table, setTable] = useState({});

    useEffect(() => {
        const fetchTable = async () => {
          const endpoint = `https://localhost:7146/api/Table/info/${id}`;
          try {
            const response = await fetch(endpoint, {
              method: "GET",
              headers: {
                "Content-Type": "application/json"
              }
            });
            if (!response.ok) {
              throw new Error('Error fetching table');
            }
            const data = await response.json();
            setTable(data);
          } catch (error) {
            console.error('Failed to fetch table:', error);
          }
        };
    
        fetchTable();
      }, []);

    return (
    <div>
        <h1>Info Table Page</h1>
        <h2>Table number: {table.number}</h2>
        <h2>Seats: {table.seats}</h2>
        <h2>Status: {table.status}</h2>
        <h2>Guests:</h2>
        <ol>
            {table.guests?.map((guest, index) => (
                <>
                <Link to={`/guests/info/${guest.guestID}`}>{index+1}. {guest.name}</Link>
                <br />
                </>
            ))}
        </ol>
    </div>
    );
};

export default TableInfoPage;
