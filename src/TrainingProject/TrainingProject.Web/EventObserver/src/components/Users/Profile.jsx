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
            errorMessage: '',
            id: '',
            userName: '',
            role: '',
            status: '',
            contactEmail: '',
            contactPhone: '',
            registrationDate: '',
            organizedEvents: 0,
            visitedEvents: 0,
            photo: '',
            emailConfirmed: false,
            emailConfirmModal: false,
            emailConfirmFail: false,
            confirmCode: '',
            myRole: AuthHelper.getRole(),
            myId: AuthHelper.getId()
        }
        this.toggleEmailConfirmModal = this.toggleEmailConfirmModal.bind(this);
        this.requestEmailConfirm = this.requestEmailConfirm.bind(this);
        this.confirmEmail = this.confirmEmail.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadData(parsed['id']);
        }
    }

    toggleEmailConfirmModal(){
        this.setState({
            emailConfirmModal: !this.state.emailConfirmModal
        });
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
            margin: '16px 0px 0px 35%',
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
            ? <span style={statusStyle}>{this.state.status}</span> : null

        const buttonPanel = this.renderButtonPanel();

        const emailConfirmedButton = this.state.myId === this.state.id && this.state.contactEmail && !this.state.emailConfirmed 
            ?
                <>
                    <Button color="warning" id="confirmEmailButton" onClick={this.requestEmailConfirm}>Подтвердить email</Button>
                    <UncontrolledTooltip placement="right" target="confirmEmailButton">
                        Для возможности восстановить пароль необходимо подтвердить email
                    </UncontrolledTooltip>
                </>
            : null;

        const confirmEmailModal = 
            <Modal isOpen={this.state.emailConfirmModal} toggle={this.toggleEmailConfirmModal}>
                <ModalHeader toggle={this.toggleEmailConfirmModal}>Подтверждение email</ModalHeader>
                <ModalBody>
                    На почту {this.state.contactEmail} выслан код подтверждения. Введите его в поле ниже, чтобы подтвердить email
                    <Input type="text" name="confirmCode" id="confirmCode" value={this.state.confirmCode} placeholder="8-значный код" onChange={this.handleInputChange} />
                    {this.state.emailConfirmFail ? <p style={{color: "red"}}>Не верный код подтверждения</p> : null}
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.confirmEmail}>Подтвердить</Button>{' '}
                    <Button color="secondary" onClick={this.toggleEmailConfirmModal}>Отмена</Button>{' '}
                    <Button color="warning" onClick={this.requestEmailConfirm}>Выслать код повторно</Button>
                </ModalFooter>
            </Modal>

        return(
        <>
        <div style={mainStyle}>
            <img style={photoStyle} src={this.state.photo} alt="user photo"/>
            <div>
                <h3>{this.state.userName}{badge}{status}</h3>
                <table cellPadding='8px'>
                    <tbody>
                        <tr><td><b>Зарегистрирован:</b></td><td>{this.state.registrationDate}</td></tr>
                        <tr><td><b>Email:</b></td><td>{this.state.contactEmail || 'Не указан'}</td><td>{emailConfirmedButton}</td></tr>
                        <tr><td><b>Телефон:</b></td><td>{this.state.contactPhone || 'Не указан'}</td></tr>
                        <tr><td><b>Организовал:</b></td><td><Link to={`/events?organizer=${this.state.id}`}>{this.state.organizedEvents} мероприятий</Link></td></tr>
                        <tr><td><b>Посетил:</b></td><td><Link to={`/events?participant=${this.state.id}`}>{this.state.visitedEvents} мероприятий</Link></td></tr>
                    </tbody>
                </table>
                {confirmEmailModal}
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
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data 
                });
            }
            else {
                this.setState({ 
                    id: data.id,
                    userName: data.userName,
                    role: data.role,
                    status: data.status,
                    contactEmail: data.contactEmail,
                    contactPhone: data.contactPhone,
                    registrationDate: data.registrationDate,
                    organizedEvents: data.organizedEvents,
                    visitedEvents: data.visitedEvents,
                    photo: data.photo,
                    emailConfirmed: data.emailConfirmed,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    requestEmailConfirm(){
        AuthHelper.fetchWithCredentials(`api/Users/${this.state.id}/confirmEmail`)
        .then((response) => {
            if (response.ok){
                this.setState({
                    emailConfirmFail: false,
                })
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            if(this.state.error)
            {
                this.setState({
                    errorMessage: data,
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}