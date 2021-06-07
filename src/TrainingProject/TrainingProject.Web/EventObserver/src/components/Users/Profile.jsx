import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Button, Badge, UncontrolledTooltip, Modal, ModalHeader, ModalBody, ModalFooter, Input } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

export default class Profile extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,

            error: false,
            noContent: false,

            id: '',
            userName: '',
            role: '',
            status: '',
            email: '',
            contactPhone: '',
            registrationDate: '',
            organizedEvents: 0,
            visitedEvents: 0,
            photo: 'img/users/default.jpg',

            myRole: AuthHelper.getRole(),
            myId: AuthHelper.getId(),
            query: window.location.search
        }
        
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    componentDidMount() {
        this.loadProfile();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.query !== window.location.search) {
            this.setState({ query: window.location.search });
            this.loadProfile();
        }
    }
    
    loadProfile() {
        const parsed = queryString.parse(window.location.search);
        if (parsed && parsed['id']) {
            this.loadData(parsed['id']);
        }
        else {
            this.props.history.push("/404")
        }
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;
        
        this.setState({
          [name]: value
        });
    }

    renderButtonPanel() {
        const buttonPanelStyle = {
            margin: '16px auto 0px',
            width: 'fit-content'
        }

        if (this.state.myId === this.state.id)
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/editProfile?id=${this.state.id}`}>Редактировать профиль</Button>{' '}
                    <Button color="primary" tag={Link} to={`/changePassword`}>Сменить пароль</Button>
                </div>
            )
        }
        else if (this.state.myRole === 'Admin' && this.state.role === 'User')
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/blocking?id=${this.state.id}`}>Управление блокировкой</Button>
                </div>
            )
        }
        else if (this.state.myRole === 'Account manager')
        {
            return(
                <div style={buttonPanelStyle}>
                    <Button color="primary" tag={Link} to={`/blocking?id=${this.state.id}`}>Управление блокировкой</Button>{' '}
                    <Button color="primary" tag={Link} to={`/changeRole?id=${this.state.id}`}>Управление ролью</Button>
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
        const statusStyle = {fontSize: '14px', color: 'red', marginLeft: '8px'};
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
        const status = (this.state.myRole === 'Admin' || this.state.myRole === 'Account manager') && this.state.status
            ? <span className="ml-2 text-danger">{this.state.status}</span> : null

        const buttonPanel = this.renderButtonPanel();

        return(
            <div className="mx-auto list-group" style={{minWidth: '750px', maxWidth: 'fit-content'}}>
                <div className="list-group-item bg-light">
                    <h3 className="m-0">Профиль пользователя</h3>
                </div>
                <div className="list-group-item">
                    <div style={mainStyle}>
                        <img style={photoStyle} src={this.state.photo} alt="user photo"/>
                        <div>
                            <h3 className="ml-2">{this.state.userName}{badge}</h3>
                            <div>{status}</div>
                            <table cellPadding='8px'>
                                <tbody>
                                    <tr><td><b>Зарегистрирован:</b></td><td>{this.state.registrationDate}</td></tr>
                                    <tr><td><b>Email:</b></td><td>{this.state.email || 'Не указан'}</td></tr>
                                    <tr><td><b>Телефон:</b></td><td>{this.state.contactPhone || 'Не указан'}</td></tr>
                                    <tr><td><b>Организовал:</b></td><td><Link to={`/events?organizerId=${this.state.id}`}>{this.state.organizedEvents} мероприятий</Link></td></tr>
                                    <tr><td><b>Посетил:</b></td><td><Link to={`/events?participantId=${this.state.id}`}>{this.state.visitedEvents} мероприятий</Link></td></tr>
                                </tbody>
                            </table>
                            {buttonPanel}
                        </div>
                    </div>
                </div>
            </div>
        )
    }

    render()
    {
        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.noContent
                ? <Alert color="info">
                    {"Пользователь удалён или ещё не зарегистрирован"}
                </Alert>
                : this.renderProfile()
            );

        return(
            <>
                {content}
            </>
        )
    }

    async loadData(userId) {
        fetch('api/users/' + userId)
        .then((response) => {
            this.setState({
                error: !response.ok,
                noContent: response.status === 204
            });
            return response.json();
        }).then((data) => {
            if (this.state.error) {
                console.log(data);
            }
            else {
                this.setState({ 
                    id: data.id,
                    userName: data.userName,
                    role: data.role,
                    status: data.status,
                    email: data.email,
                    contactPhone: data.contactPhone,
                    registrationDate: data.registrationDate,
                    organizedEvents: data.organizedEvents,
                    visitedEvents: data.visitedEvents,
                    photo: data.photo,

                    loading: false
                });
            }
        }).catch((ex) => {
            console.log(ex.toString());
        }).finally(() => {
            this.setState({
                loading: false
            });
        });
    }
}