import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import alertError from '../../alertError';

const TableInfoPage = () => {
  const navigate = useNavigate();
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
          const message = await alertError(response);
          throw new Error(message);
        }
        const data = await response.json();
        setTable(data);
      } catch (error) {
        console.error('Failed to fetch table:', error);
      }
    };

    fetchTable();
  }, []);

  const deleteTable = async (tableID) => {
    try {
      const endpoint = `https://localhost:7146/api/Table/${tableID}`;
      const response = await fetch(endpoint, {
        method: 'DELETE',
      });
      if (!response.ok) {
        const message = await alertError(response);
        throw new Error(message);
      }
      navigate('/tables')
    } catch (error) {
      console.error('Failed to delete table:', error);
    }
  };

  return (
  <div>
      <h1>Info Table Page</h1>
      <h2>Table number: {table.number}</h2>
      <h2>Seats: {table.seats}</h2>
      <h2>Status: {table.status}</h2>
      <Link to={`/tables/edit/${table.tableID}`}>Edit</Link>
      <a
        style={{ color: 'blue', textDecoration: 'underline', cursor: 'pointer' }}
        onClick={() => {deleteTable(table.tableID)}}
      >
      Delete
      </a>
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
