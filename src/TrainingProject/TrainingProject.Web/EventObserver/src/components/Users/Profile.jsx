import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Button, Badge } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

export default class Profile extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,
            error: false,
            errorMessage: '',
            id: null,
            userName: '',
            role: '',
            status: '',
            contactEmail: '',
            contactPhone: '',
            registrationDate: '',
            organizedEvents: 0,
            visitedEvents: 0,
            photo: '',
            myRole: AuthHelper.getRole(),
            myId: AuthHelper.getId()
        }
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadData(parsed['id']);
        }
    }

    renderButtonPanel() {
        const buttonPanelStyle = {
            margin: '16px 0px 0px 35%',
        }

        if (this.state.myId == this.state.id)
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/editProfile?id=${this.state.id}`}>Редактировать профиль</Button>{' '}
                    <Button color="primary" tag={Link} to={`/changePassword?id=${this.state.id}`}>Сменить пароль</Button>
                </div>
            )
        }
        else if (this.state.myRole == 'Admin' && this.state.role == 'User')
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/blocking?id=${this.state.id}`}>Управление блокировкой</Button>
                </div>
            )
        }
        else if (this.state.myRole == 'Account manager')
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/blocking?id=${this.state.id}`}>Управление блокировкой</Button>{' '}
                    <Button color="primary" tag={Link} to={`/roles?id=${this.state.id}`}>Управление ролью</Button>
                </div>
            )
        }
        else{
            return null;
        }
    }

    renderProfile()
    {
        const mainStyle = {display: 'flex'};
        const photoStyle = {maxWidth: '300px', marginRight: '16px'};
        const statusStyle = {fontSize: '14px', color: 'red'};
        const bageStyle = {marginLeft: '8px'};

        let badge;
        switch(this.state.role)
        {
            case 'Account manager':
                badge = <Badge style={bageStyle} color="success">Account manager</Badge>
                break;
            case 'Admin': 
                badge = <Badge style={bageStyle} color="danger">Admin</Badge>
                break;
            default:
                badge = null;
        }
        const status = (this.state.myRole == 'Admin' || this.state.myRole == 'Account manager') && this.state.status
            ? <span style={statusStyle}>{this.state.status}</span> : null

        const buttonPanel = this.renderButtonPanel();

        return(
        <>
        <div style={mainStyle}>
            <img style={photoStyle} src={this.state.photo} alt="user photo"/>
            <div>
                <h3>{this.state.userName}{badge}{status}</h3>
                <table cellPadding='8px'>
                    <tbody>
                        <tr><td><b>Зарегистрирован:</b></td><td>{this.state.registrationDate}</td></tr>
                        <tr><td><b>Email:</b></td><td>{this.state.contactEmail}</td></tr>
                        <tr><td><b>Телефон:</b></td><td>{this.state.contactPhone}</td></tr>
                        <tr><td><b>Организовал:</b></td><td><Link to={`/events?organizer=${this.state.id}`}>{this.state.organizedEvents} мероприятий</Link></td></tr>
                        <tr><td><b>Посетил:</b></td><td><Link to={`/events?participant=${this.state.id}`}>{this.state.visitedEvents} мероприятий</Link></td></tr>
                    </tbody>
                </table>
            </div>
        </div>
        {buttonPanel}
        </>)
    }

    render()
    {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderProfile();

        return(
            <>
                {errorBaner}
                {content}
            </>
        )
    }

    async loadData(userId) {
        fetch('api/Users/' + userId)
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
                    id: data.id,
                    userName: data.userName,
                    role: data.role,
                    status: data.status,
                    contactEmail: data.contactEmail || 'Не указан',
                    contactPhone: data.contactPhone || 'Не указан',
                    registrationDate: data.registrationDate,
                    organizedEvents: data.organizedEvents,
                    visitedEvents: data.visitedEvents,
                    photo: data.photo,
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