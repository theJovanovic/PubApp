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
    <div className="page-container">

      <div className="title-container">
        <h1>Info Table Page</h1>
      </div>

      <div class="main-container">
        <div className="helper-container"></div>

        <div className="info-container">
            <div className="info-number-container">
              <h2>Table number: {table.number}</h2>
            </div>

            <div className="info-seats-container">
              <h2>Seats: {table.seats}</h2>
            </div>

            <div className="info-status-container">
              <h2>Status: {table.status}</h2>
            </div>
            
            <div className="info-button-container">
              <div className="helper-container"></div>
              <Link to={`/tables/edit/${table.tableID}`} className="button-edit">Edit</Link>
              <button className="button-delete" onClick={() => deleteTable(table.tableID)}>Delete</button>
              <div className="helper-container"></div>
            </div>
        </div>
        
        <div className="helper-container"></div>
      </div>

      <div className="tableguests-container">

        <div className="tableguests-title">
          <h1>Guests:</h1>
        </div>

        <div className="tableguests-list">
            {table.guests?.map((guest, index) => (
                <div className="tableguest-container">
                  <Link id={`guest_${guest.guestID}`} to={`/guests/info/${guest.guestID}`}>{index+1}. {guest.name}</Link>
                </div>
            ))}
        </div>
      </div>

    </div>
  );
};

export default TableInfoPage;
