import React from 'react';
import { Link } from 'react-router-dom';

const NavigationHeader = () => {
  return (
    <header>
      <nav>
        <ul>
          <li>
            <Link to="/tables">Tables</Link>
          </li>
          <li>
            <Link to="/guests">Guests</Link>
          </li>
          {/* Add more navigation links here as needed */}
        </ul>
      </nav>
    </header>
  );
};

export default NavigationHeader;
