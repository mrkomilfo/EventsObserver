import React, { Component } from 'react';
import {
    Button,
    Form,
    FormGroup,
    Label,
    Input,
    FormFeedback,
    Alert,
    Modal,
    ModalHeader, ModalBody, ModalFooter
} from 'reactstrap';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            error: false,
            errorMessage: '',

            name: '', 
            email: '', 
            phone: '',
            password: '', 
            passwordConfirm: '',
            formErrors: { 
                name: '', 
                email: '', 
                phone: '',
                password: '',
                passwordConfirm: '' 
            },
            formValid: false, 
            nameValid: false, 
            emailValid: false, 
            phoneValid: true,
            passwordValid: false, 
            passwordConfirmValid: false,

            emailConfirmModal: false,
            emailConfirmErrorMessage: '',
            confirmCode: '',
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.register = this.register.bind(this);
        this.toggleEmailConfirmModal = this.toggleEmailConfirmModal.bind(this);
        this.confirmEmail = this.confirmEmail.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;

        this.setState({
          [name]: value}, 
          () => { 
              this.validateField(name, value) 
            }
        );
    }

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;

        let nameValid = this.state.nameValid;
        let emailValid = this.state.emailValid;
        let phoneValid = this.state.phoneValid;
        let passwordValid = this.state.passwordValid;
        let passwordConfirmValid = this.state.passwordConfirmValid;

        switch(fieldName) {
            case 'name':
                nameValid = value.length >= 4;
                fieldValidationErrors.name = nameValid ? '' : 'Минимальная длина - 4';
                break;
            case 'email':
                emailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i);
                fieldValidationErrors.email = emailValid ? '' : 'Неверный формат';
                break;
            case 'phone':
                phoneValid = value.match(/^\+?[0-9]{6,12}$/i) || value.length === 0;
                fieldValidationErrors.phone = phoneValid ? '' : 'Неверный формат';
                break;
            case 'password':
                passwordValid = value.length >= 6;
                fieldValidationErrors.password = passwordValid ? '' : 'Минимальная длина - 6';
                passwordConfirmValid = this.state.passwordConfirm === this.state.password;
                fieldValidationErrors.passwordConfirm = passwordConfirmValid ? '' : 'Пароли не совпадают';
                break;
            case 'passwordConfirm':
                passwordConfirmValid = value === this.state.password;
                fieldValidationErrors.passwordConfirm = passwordConfirmValid ? '' : 'Пароли не совпадают';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            nameValid,
            emailValid,
            phoneValid,
            passwordValid,
            passwordConfirmValid
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.nameValid &&
                this.state.emailValid &&
                this.state.phoneValid &&
                this.state.passwordValid &&
                this.state.passwordConfirmValid
        });
    }

    toggleEmailConfirmModal() {
        this.setState({
            emailConfirmModal: !this.state.emailConfirmModal
        });
    }

    renderConfirmEmailModal() {
        return(
            <Modal isOpen={this.state.emailConfirmModal} toggle={this.toggleEmailConfirmModal}>
                <ModalHeader toggle={this.toggleEmailConfirmModal}>Подтверждение email</ModalHeader>
                <ModalBody>
                    <p>На почту <b>{this.state.email}</b> выслан код подтверждения. Введите его в поле ниже, чтобы подтвердить email. Код действителен 5 минут.</p>
                    <Input type="text" name="confirmCode" id="confirmCode" value={this.state.confirmCode} placeholder="8-значный код" onChange={this.handleInputChange} />
                    <span style={{color: "red"}}>{this.state.emailConfirmErrorMessage}</span>
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.confirmEmail}>Подтвердить</Button>{' '}
                    <Button color="secondary" onClick={this.toggleEmailConfirmModal}>Отмена</Button>{' '}
                    <Button color="warning" onClick={this.requestEmailConfirm}>Выслать код повторно</Button>
                </ModalFooter>
            </Modal>
        )
    }

    render(){
        const errorBanner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const formStyle = {
            maxWidth: '256px'
        }

        return(
            <div className="mx-auto" style={{maxWidth: '300px'}}>
                {errorBanner}
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Регистрация</h3>
                    </div>
                    <div className="list-group-item">
                        <Form style={formStyle}>
                            <FormGroup>
                                <Label for="name">Имя пользователя</Label>
                                <Input invalid={!this.state.nameValid} required type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.name}</FormFeedback>
                            </FormGroup>
                            <FormGroup>
                                <Label for="email">Email</Label>
                                <Input invalid={!this.state.emailValid} type="email" name="email" id="email" value={this.state.email} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.email}</FormFeedback>
                            </FormGroup>
                            <FormGroup>
                                <Label for="password">Пароль</Label>
                                <Input invalid={!this.state.passwordValid} required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.password}</FormFeedback>
                            </FormGroup>
                            <FormGroup>
                                <Label for="passwordConfirm">Повторите пароль</Label>
                                <Input invalid={!this.state.passwordConfirmValid} required type="password" name="passwordConfirm" id="passwordConfirm" value={this.state.passwordConfirm} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.passwordConfirm}</FormFeedback>
                            </FormGroup>
                            <FormGroup>
                                <Label for="phone">Телефон</Label>
                                <Input invalid={!this.state.phoneValid} type="tel" name="phone" id="phone" value={this.state.phone} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.phone}</FormFeedback>
                            </FormGroup>
                            <Button color="primary" disabled = {!this.state.formValid} onClick={() => this.register()}>Зарегистрироваться</Button>
                        </Form>
                    </div>
                </div>
                {this.renderConfirmEmailModal()}
            </div>
        )
    }

    register() {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let data = {
            userName: this.state.name,
            email: this.state.email,
            password: this.state.password, 
            passwordConfirm: this.state.passwordConfirm,
            contactPhone: this.state.phone
        };

        fetch('api/users', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok) {
                this.requestEmailConfirm();
            }
            else {
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            if(this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    requestEmailConfirm() {
        fetch(`api/users/${this.state.email}/confirmEmail`)
            .then((response) => {
                if (response.ok) {
                    this.setState({
                        emailConfirmErrorMessage: '',
                        emailConfirmModal: true
                    })
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
                emailConfirmErrorMessage: 'Неверный код подтверждения'
            });
            return;
        }

        fetch(`api/users/${this.state.email}/confirmEmail?confirmCode=${this.state.confirmCode}`, {
            method: 'PUT'
        }).then((response) => {
            if (response.ok) {
                this.props.history.push("/signIn");
            }
            else if (response.status === 400) {
                this.setState({
                    emailConfirmErrorMessage: 'Неверный код подтверждения'
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