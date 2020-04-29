import React, { Component } from 'react';
import { Media } from 'reactstrap';
import { Link } from 'react-router-dom';

export default class EventMedia extends Component {
    render() {
        const imgStyle = {
            objectFit: 'cover',
            maxHeight: 140,
            maxWidth: 256,
            marginRight: '10px'
        }

        let image = this.props.event.image ? 
        <Media left>
            <Media object src={this.props.event.image} alt="Image" style={imgStyle}/>
        </Media> : null;
        
        return(
        <Media>
            {image}
            <Media body>
                <Media heading>
                    <Link to={`/event?id=${this.props.event.id}`}>{this.props.event.name}</Link>                  
                </Media>
                {`Категория:`} <Link to={`/events?categoryId=${this.props.event.categoryId}`}>{this.props.event.category}</Link>
                <br/>
                {`Место: ${this.props.event.place}`}
                <br/>
                {`Старт: ${this.props.event.start}`}
                <br/>
                {`Взнос: ${this.props.event.fee ? this.props.event.fee +'BYN' : 'Бесплатно'}`}
            </Media>
        </Media>)
    }
}
