import React, { Component } from 'react';

import './404.css';

export default class Page404 extends Component
{
    render(){
        return(
            <div className="overlay404">
                <div className="body404">
                    <p className="top">404</p>
                    <p className="body">Page not found</p>
                </div>
            </div>
        )
    }
}