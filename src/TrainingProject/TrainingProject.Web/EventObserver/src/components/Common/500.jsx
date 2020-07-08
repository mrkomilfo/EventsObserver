import React, { Component } from 'react';

import './500.css';

export default class Page500 extends Component
{
    render() {
        return(
            <div className="overlay500">
                <div className="body500">
                    <p className="number500">500</p>
                    <p className="inscription500">Internal server error</p>
                </div>
            </div>
        )
    }
}