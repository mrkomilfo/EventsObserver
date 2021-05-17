import React, { Component } from 'react';
import { Link } from 'react-router-dom';

export default class EventRow extends Component {
    render() {
        const imgStyle = {
            objectFit: 'cover',
            maxHeight: 150,
            maxWidth: 300,
        }

        let image = this.props.event.image 
            ? <Link to={`/event?id=${this.props.event.id}`}>
                <img src={this.props.event.image} alt="Image" style={imgStyle}/>
            </Link> 
            : null;

        return(
            <tr>
                <td>
                    {image}
                </td>
                <td className="pt-0">
                    <table className="table table-sm table-borderless m-0">
                        <tbody>
                            <tr>
                                <td colSpan="2">
                                    <h4 className="m-0">
                                        <Link style={{ wordBreak: "break-all" }} to={`/event?id=${this.props.event.id}`}>
                                            {this.props.event.name}
                                        </Link>
                                    </h4>
                                </td>
                            </tr>
                            <tr>
                                <td>Категория:</td>
                                <td><Link to={`/events?categoryId=${this.props.event.categoryId}`}>{this.props.event.category}</Link></td>
                            </tr>
                            <tr>
                                <td>Место:</td>
                                <td>{this.props.event.place}</td>
                            </tr>
                            <tr>
                                <td>Начало:</td>
                                <td>{this.props.event.start}</td>
                            </tr>
                            <tr>
                                <td>Стоимость:</td>
                                <td>{this.props.event.fee ? this.props.event.fee +'BYN' : 'Бесплатно'}</td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        )
    }
}
