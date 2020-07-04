import React, { Component } from 'react';

import './403.css';

export default class Page403 extends Component
{
    render() {
        return(
            <div className="overlay403">
                <div className="body403">
                    <p className="number403">403</p>
                    <p className="inscription403">Forbidden</p>
                </div>
            </div>
        )
    }
}