import React, { Component } from 'react';
import queryString from 'query-string';
import { Button, Form, FormGroup, Label, Input, Alert, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            email: '', 
            password: '', 
            errorMessage: '', 
            error: false,
            resetEmail: '',
            confirmCode: '',
            requestResetCodeModal: false,
            resetPasswordModal: false,
            resetPasswordConfirmModal: false,
            requestResetCodeErrorMessage: '',
            resetPasswordErrorMessage: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.email = this.email.bind(this);
        this.enterPressed = this.enterPressed.bind(this);
        this.toggleRequestResetCodeModal = this.toggleRequestResetCodeModal.bind(this);
        this.toggleResetPasswordModal = this.toggleResetPasswordModal.bind(this);
        this.toggleResetPasswordConfirmModal = this.toggleResetPasswordConfirmModal.bind(this);
        this.requestResetCode = this.requestResetCode.bind(this);
        this.resetPassword = this.resetPassword.bind(this);
    }

    toggleRequestResetCodeModal() {
        this.setState({
            requestResetCodeModal: !this.state.requestResetCodeModal
        });
    }

    toggleResetPasswordModal() {
        this.setState({
            resetPasswordModal: !this.state.resetPasswordModal
        });
    }

    toggleResetPasswordConfirmModal() {
        this.setState({
            resetPasswordConfirmModal: !this.state.resetPasswordConfirmModal
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

    enterPressed(event) {
        var code = event.keyCode || event.which;
        if (code === 13) {
            this.email();
        }
    };

    render() {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const requestResetCodeModal = 
            <Modal isOpen={this.state.requestResetCodeModal} toggle={this.toggleRequestResetCodeModal}>
                <ModalHeader toggle={this.toggleRequestResetCodeModal}>Сброс пароля</ModalHeader>
                <ModalBody>
                    <p>Введите ваш email в поле ниже.</p>
                    <Input type="email" name="resetEmail" id="resetEmail" value={this.state.resetEmail} placeholder="Логин" onChange={this.handleInputChange} />
                    <span style={{color: "red"}}>{this.state.requestResetCodeErrorMessage}</span>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.requestResetCode}>Получить код</Button>{' '}
                    <Button color="secondary" onClick={this.toggleRequestResetCodeModal}>Отмена</Button>
                </ModalFooter>
            </Modal>

        const resetPasswordModal = 
            <Modal isOpen={this.state.resetPasswordModal} toggle={this.toggleResetPasswordModal}>
                <ModalHeader toggle={this.toggleResetPasswordModal}>Сброс пароля</ModalHeader>
                <ModalBody>
                    <p>На вашу почту выслан код подтверждения. Код действителен 5 минут.</p>
                    <Input type="text" name="confirmCode" id="confirmCode" value={this.state.confirmCode} placeholder="8-значный код" onChange={this.handleInputChange} />
                    <span style={{color: "red"}}>{this.state.resetPasswordErrorMessage}</span>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.resetPassword}>Сбросить пароль</Button>{' '}
                    <Button color="secondary" onClick={this.toggleResetPasswordModal}>Отмена</Button>
                </ModalFooter>
            </Modal>

        const resetPasswordConfirmModal = 
            <Modal isOpen={this.state.resetPasswordConfirmModal} toggle={this.toggleResetPasswordConfirmModal}>
                <ModalHeader toggle={this.toggleResetPasswordConfirmModal}>Сброс пароля</ModalHeader>
                <ModalBody>
                    На вашу почту выслан новый пароль. Пожалуйста, смените его при первом входе.
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.toggleResetPasswordConfirmModal}>Ok</Button>
                </ModalFooter>
            </Modal>

        return( 
            <div className="mx-auto" style={{maxWidth: '320px'}}>
                {errorBaner}
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Вход</h3>
                    </div>
                    <div className="list-group-item">
                        <Form>    
                            <FormGroup>
                                <Label for="email">Email</Label>
                                <Input required type="text" name="email" id="email" value={this.state.email} onChange={this.handleInputChange} onKeyPress={this.enterPressed}/>
                            </FormGroup>
                            <FormGroup>
                                <Label for="password">Пароль</Label>
                                <Input required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange} onKeyPress={this.enterPressed}/>
                            </FormGroup>
                            <div>
                                <Button color="primary" onClick={this.email}>Войти</Button>{' '}
                                <Button color="warning" onClick={this.toggleRequestResetCodeModal}>Я забыл пароль</Button>
                            </div>
                            {requestResetCodeModal}
                            {resetPasswordModal}
                            {resetPasswordConfirmModal}
                        </Form>
                    </div>
                </div>
            </div>
        )
    }

    email() {
        let auth_data = {
            email: this.state.email,
            password: this.state.password
        };
        fetch('api/users/signIn', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(auth_data)
        }).then((response) => {
            this.setState({
                error: !response.ok
            });
            if (response.status === 401)
            {
                this.setState({
                    errorMessage: 'Неверный email или пароль'
                })
            }
            else if (response.status === 403)
            {
                fetch(`api/users/blockingExpiration?email=${this.state.email}`)
                    .then(response=>response.text())
                    .then(data => {
                        this.setState({
                            errorMessage: `Заблокирован до ${data}`
                        })
                    });
            }
            return response.json();
        }).then((data) => {
            if(this.state.error) {
                console.log(data);
            }
            else {
                AuthHelper.saveAuth(data.name, data.role, data.accessToken, data.refreshToken);
                const parsed = queryString.parse(window.location.search);
                if (parsed && parsed['goBack'] && parsed['goBack'] === 'false') {
                    this.props.history.push('/events');
                }
                else {
                    this.props.history.goBack();
                }
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    requestResetCode() {
        if (!this.state.resetEmail.trim())
        {
            this.setState({
                requestResetCodeErrorMessage: 'Пользователь с таким email не существует'
            });
            return;
        }
        fetch(`api/users/resetPassword?email=${this.state.resetEmail}`)
        .then((response) => {
            if (response.ok) {
                this.setState({
                    requestResetCodeErrorMessage: '',
                    resetPasswordModal: true
                })
            }
            else if (response.status === 204) {
                this.setState({
                    requestResetCodeErrorMessage: 'Пользователь с таким email не существует'
                });
            }
            else if (response.status === 403) {
                this.setState({
                    requestResetCodeErrorMessage: 'Отсутствует подтверждённая почта'
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

    resetPassword() {
        if (!this.state.confirmCode.trim() || this.state.confirmCode.trim().length !== 8)
        {
            this.setState({
                resetPasswordErrorMessage: 'Не верный код подтверждения'
            });
            return;
        }
        fetch(`api/users/resetPassword?email=${this.state.resetEmail}&confirmCode=${this.state.confirmCode}`, {
            method: 'PUT'
        }).then((response) => {
            if (response.ok) {
                this.setState({
                    resetPasswordErrorMessage: '',
                    resetPasswordModal: false,
                    requestResetCodeModal: false,
                    resetPasswordConfirmModal: true
                })
            }
            else if (response.status === 204) {
                this.setState({
                    resetPasswordErrorMessage: 'Пользователь с таким email не существует'
                });
            }
            else if (response.status === 401) {
                this.setState({
                    resetPasswordErrorMessage: 'Не верный код подтверждения'
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