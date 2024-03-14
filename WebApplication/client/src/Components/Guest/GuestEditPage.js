import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const GuestEditPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [guest, setGuest] = useState({});

  useEffect(() => {
    const fetchGuest = async () => {
      const endpoint = `https://localhost:7146/api/Guest/${id}`;
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

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setGuest((prevGuest) => ({
      ...prevGuest,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    console.log('Submitting guest:', guest);
    try {
      const endpoint = `https://localhost:7146/api/Guest/${id}`;
      const response = await fetch(endpoint, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(guest),
      });
      if (!response.ok) {
        if (response.status === 404) {
          alert("Error: Table doesn't exist")
        }
        throw new Error('Error editing guest');
      }
      navigate(`/guests/info/${id}`);
      // navigate(`/guests`);
    } catch (error) {
      console.error('Failed to edit guest:', error);
    }
  };

  return (
    <div>
      <h1>Edit Guest Page</h1>
      <form onSubmit={handleSubmit}>
      <label>
          Name:
          <input
            type="text"
            name="name"
            value={guest.name}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Money:
          <input
            type="number"
            name="money"
            value={guest.money}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <label>
          Has allergies:
          <input
            type="checkbox"
            name="hasAllergies"
            checked={!!guest.hasAllergies}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Has discount:
          <input
            type="checkbox"
            name="hasDiscount"
            checked={!!guest.hasDiscount}
            onChange={handleChange}
          />
        </label>
        <br />
        <label>
          Table number:
          <input
            type="number"
            name="tableNumber"
            value={guest.tableNumber}
            onChange={handleChange}
            required
          />
        </label>
        <br />
        <button type="submit">Edit Guest</button>
      </form>
    </div>
  );
};

export default GuestEditPage;
