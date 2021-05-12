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
            contactEmail: '',
            contactPhone: '',
            registrationDate: '',
            organizedEvents: 0,
            visitedEvents: 0,
            photo: 'img/users/default.jpg',
            emailConfirmed: false,
            emailConfirmModal: false,
            emailConfirmErrorMessage: '',
            confirmCode: '',
            myRole: AuthHelper.getRole(),
            myId: AuthHelper.getId(),
            query: window.location.search
        }
        this.toggleEmailConfirmModal = this.toggleEmailConfirmModal.bind(this);
        this.requestEmailConfirm = this.requestEmailConfirm.bind(this);
        this.confirmEmail = this.confirmEmail.bind(this);
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

    toggleEmailConfirmModal() {
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
                    <p>На почту <b>{this.state.contactEmail}</b> выслан код подтверждения. Введите его в поле ниже, чтобы подтвердить email. Код действителен 5 минут.</p>
                    <Input type="text" name="confirmCode" id="confirmCode" value={this.state.confirmCode} placeholder="8-значный код" onChange={this.handleInputChange} />
                    <span style={{color: "red"}}>{this.state.emailConfirmErrorMessage}</span>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.confirmEmail}>Подтвердить</Button>{' '}
                    <Button color="secondary" onClick={this.toggleEmailConfirmModal}>Отмена</Button>{' '}
                    <Button color="warning" onClick={this.requestEmailConfirm}>Выслать код повторно</Button>
                </ModalFooter>
            </Modal>

        return(
            <div className="mx-auto list-group" style={{maxWidth: '720px'}}>
                <div className="list-group-item bg-light">
                    <h3 className="m-0">Профиль пользователя</h3>
                </div>
                <div className="list-group-item">
                    <div style={mainStyle}>
                        <img style={photoStyle} src={this.state.photo} alt="user photo"/>
                        <div>
                            <h3>{this.state.userName}{badge}{status}</h3>
                            <table cellPadding='8px'>
                                <tbody>
                                    <tr><td><b>Зарегистрирован:</b></td><td>{this.state.registrationDate}</td></tr>
                                    <tr><td><b>Email:</b></td><td>{this.state.contactEmail || 'Не указан'}</td><td>{emailConfirmedButton}</td></tr>
                                    <tr><td><b>Телефон:</b></td><td>{this.state.contactPhone || 'Не указан'}</td></tr>
                                    <tr><td><b>Организовал:</b></td><td><Link to={`/events?organizerId=${this.state.id}`}>{this.state.organizedEvents} мероприятий</Link></td></tr>
                                    <tr><td><b>Посетил:</b></td><td><Link to={`/events?participantId=${this.state.id}`}>{this.state.visitedEvents} мероприятий</Link></td></tr>
                                </tbody>
                            </table>
                            {buttonPanel}
                            {confirmEmailModal}
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
        fetch('api/Users/' + userId)
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
            console.log(ex.toString());
        }).finally(() => {
            this.setState({
                loading: false
            });
        });
    }

    requestEmailConfirm() {
        AuthHelper.fetchWithCredentials(`api/Users/${this.state.id}/confirmEmail`)
        .then((response) => {
            if (response.ok) {
                this.setState({
                    emailConfirmErrorMessage: '',
                    emailConfirmModal: true
                })
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                this.setState({
                    error: true
                });
                return response.json();
            }
        }).then((data) => {
            if (this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    confirmEmail() {
        if (!this.state.confirmCode.trim() || this.state.confirmCode.trim().length != 8)
        {
            this.setState({
                emailConfirmErrorMessage: 'Не верный код подтверждения'
            });
            return;
        }
        AuthHelper.fetchWithCredentials(`api/Users/${this.state.id}/confirmEmail?confirmCode=${this.state.confirmCode}`, {
            method: 'PUT'
        }).then((response) => {
            if (response.ok) {
                this.setState({
                    emailConfirmErrorMessage: '',
                    emailConfirmModal: false,
                    emailConfirmed: true
                })
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else if (response.status === 400) {
                this.setState({
                    emailConfirmErrorMessage: 'Не верный код подтверждения'
                });
            }
            else {
                this.setState({
                    error: true
                });
                return response.json();
            }
        }).then((data) => {
            if (this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}