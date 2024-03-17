import React from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Navbar.css';

const NavigationHeader = () => {
  return (
  <Navbar expand="md" className="sticky-top navbar-bg-custom-color" > 
        <Navbar.Brand href="/tables" className="navbar-brand-custom-color">Stefan's PUB</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav" className=".navbar-collapse-centered">
          <Nav className="nav-centered">
            <Nav.Link href="/tables" className="nav-link-custom-color">Tables</Nav.Link>
            <Nav.Link href="/guests" className="nav-link-custom-color">Guests</Nav.Link>
            <Nav.Link href="/menu" className="nav-link-custom-color">Menu</Nav.Link>
            <Nav.Link href="/waiters" className="nav-link-custom-color">Waiters</Nav.Link>
          </Nav>
        </Navbar.Collapse>
    </Navbar>
  );
}

export default NavigationHeader;
