import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

export default class EventMedia extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,
            name: '',
            categoryId: null,
            category: '',
            description: '',
            start: '',
            place: '',
            fee: 0,
            participantsLimit: 0,
            organizerId: '',
            organizer: '',
            participants: {},
            tags: {},
            publicationTime: '',
            image: '',
            userRole: AuthHelper.getRole(),
            userId: AuthHelper.getId()
        }
        //this.deleteEvent = this.deleteEvent.bind(this);
    }
    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadData(parsed['id']);
        }
    }

    renderEvent()
    {
        const headerStyle = {display: 'table'};
        const nameStyle = {display: 'table-cell'};
        const publishedStyle = {marginLeft: '12px'};
        const imageStyle = {width: '100%'};
        const tagBlockStyle = {display: 'inline'}

        const image = this.state.image 
            ? <img style={imageStyle} src={this.state.image} alt="event image"/> 
            : null;

        let tags = Object.keys(this.state.tags).map((key, index) => {
            return (
                <Link className="link" to={"/events?tag=" + this.state.tags[key]} key={key}>#{this.state.tags[key]} </Link>
            );
        });

        return(
            <>
                <div style={headerStyle}><h2 style={nameStyle}>{this.state.name}</h2><p style={publishedStyle}>Опубликовано: {this.state.publicationTime}</p></div>
                <h4>Категория: <Link to={`/events?categoryId=${this.state.categoryId}`}>{this.state.category}</Link></h4>
                {image}
                <p>{this.state.description}</p>
                <table cellPadding='8px'>
                    <tr><td><b>Место проведения:</b></td><td>{this.state.place}</td></tr>
                    <tr><td><b>Начало:</b></td><td>{this.state.start}</td></tr>
                    <tr><td><b>Взнос:</b></td><td>{this.state.fee ? this.state.fee +'BYN' : 'Бесплатно'}</td></tr>
                </table>
                <div style={tagBlockStyle}>{tags}</div>
            </>
        ) 
    }

    render()
    {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEvent();

        return(
            <>
            {errorBaner}
            {content}
            </>
        )
    }

    async loadData(eventId) {
        fetch('api/Events/' + eventId)
        .then((response) => {
            if (!response.ok) {
                this.setState({
                    error: true
                });
            }
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data.message 
                });
            }
            else {
                this.setState({ 
                    name: data.name,
                    categoryId: data.categoryId,
                    category: data.category,
                    description: data.description,
                    start: data.start,
                    place: data.place,
                    fee: data.fee,
                    participantsLimit: data.participantsLimit,
                    organizerId: data.organizerId,
                    organizer: data.organizer,
                    participants: data.participants,
                    tags: data.tags,
                    publicationTime: data.publicationTime,
                    image: data.image,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}