import React from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import 'bootstrap/dist/css/bootstrap.min.css';

const NavigationHeader = () => {
  return (
  <Navbar expand="sm" className="navbar-bg-custom-color" > 
      <Container>
        <Navbar.Brand href="/tables" className="navbar-brand-custom-color">PUB - Stefan je car</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="">
            <Nav.Link href="/tables" className="nav-link-custom-color">Tables</Nav.Link>
            <Nav.Link href="/guests" className="nav-link-custom-color">Guests</Nav.Link>
            <Nav.Link href="/menu" className="nav-link-custom-color">Menu</Nav.Link>
            <Nav.Link href="/waiters" className="nav-link-custom-color">Waiters</Nav.Link>
            {/* <Nav.Link href="/guests" className="nav-link-custom-color">Orders</Nav.Link> */}
            {/* <NavDropdown title="Dropdown" id="basic-nav-dropdown">
              <NavDropdown.Item href="#action/3.1">Action</NavDropdown.Item>
              <NavDropdown.Item href="#action/3.2">
                Another action
              </NavDropdown.Item>
              <NavDropdown.Item href="#action/3.3">Something</NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Item href="#action/3.4">
                Separated link
              </NavDropdown.Item>
            </NavDropdown> */}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavigationHeader;
