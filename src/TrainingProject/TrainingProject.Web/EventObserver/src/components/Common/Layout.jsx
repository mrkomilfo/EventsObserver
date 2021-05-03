import React, { Component } from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

export default class Layout extends Component {
  static displayName = Layout.name;

  render() {
    return ( 
      <div>
        <NavMenu />
        <Container style={{marginBottom: '12px', width: 'fit-content'}}>
          {this.props.children}
        </Container>
      </div>
    );
  }
}