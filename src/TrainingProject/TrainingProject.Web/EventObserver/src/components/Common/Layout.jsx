import React, { Component } from 'react';
import NavMenu from './NavMenu';

export default class Layout extends Component {
  static displayName = Layout.name;

  render() {
    return ( 
      <div>
          <NavMenu />
          <div className="container mb-3">
              {this.props.children}
          </div>
      </div>
    );
  }
}