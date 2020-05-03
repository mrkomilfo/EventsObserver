import React, { Component } from 'react';

export default class Error404 extends Component
{
    render(){
        const topStyle = {
            textAlign: 'center',
            fontSize: '128px'
        }

        const bottomStyle = {
            textAlign: 'center',
            fontSize: '64px'
        }
        return(
            <div>
                <p style={topStyle}>404</p>
                <p style={bottomStyle}>Страница не найдена</p>
            </div>
        )
    }
}