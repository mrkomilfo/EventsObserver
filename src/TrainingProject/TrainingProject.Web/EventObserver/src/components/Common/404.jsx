import React, { Component } from 'react';

import './404.css';

export default class Page404 extends Component
{
    render() {
        return(
            <div className="overlay404">
                <div className="body404">
                    <p className="number404">404</p>
                    <p className="inscription404">Page not found</p>
                </div>
            </div>
        )
    }
}