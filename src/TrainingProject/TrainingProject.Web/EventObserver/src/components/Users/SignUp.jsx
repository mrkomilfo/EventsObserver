import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            name: '', email: '', phone: '', login: '', password: '', passwordConfirm: '',
            formErrors: { name: '', email: '', phone: '', login: '', password: '', passwordConfirm: '' },
            nameValid: false, emailValid: true, phoneValid: true, loginValid: false, passwordValid: false, passwordConfirmValid: false,
            errorMessage: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.register = this.register.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;

        this.setState({
          [name]: value}, 
          () => { this.validateField(name, value) }
        );
    }

    validateField(fieldName, value){
        let fieldValidationErrors = this.state.formErrors;

        let nameValid = this.state.nameValid;
        let emailValid = this.state.emailValid;
        let phoneValid = this.state.phoneValid;
        let loginValid = this.state.loginValid;
        let passwordValid = this.state.passwordValid;
        let passwordConfirmValid = this.state.passwordConfirmValid;

        switch(fieldName){
            case 'name':
                nameValid = value.length >= 4;
                fieldValidationErrors.name = nameValid ? '' : 'Минимальная длина - 4';
                break;
            case 'email':
                emailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i) || value.length == 0;
                fieldValidationErrors.email = emailValid ? '' : 'Неверный формат';
                break;
            case 'phone':
                phoneValid = value.match(/^[\+]?[0-9]{6,12}$/i) || value.length == 0;
                fieldValidationErrors.phone = phoneValid ? '' : 'Неверный формат';
                break;
            case 'login':
                loginValid = value.length >= 4;
                fieldValidationErrors.login = loginValid ? '' : 'Минимальная длина - 4';
                break;
            case 'password':
                passwordValid = value.length >= 6;
                fieldValidationErrors.password = passwordValid ? '' : 'Минимальная длина - 6';
                break;
            case 'passwordConfirm':
                passwordConfirmValid = value == this.state.password;
                fieldValidationErrors.passwordConfirm = passwordConfirmValid ? '' : 'Пароли не совпадают';
                break;
            default:
                break;
        }
        this.setState({formErrors: fieldValidationErrors,
            nameValid: nameValid,
            emailValid: emailValid,
            phoneValid: phoneValid,
            loginValid: loginValid,
            passwordValid: passwordValid,
            passwordConfirmValid: passwordConfirmValid
          }, this.validateForm);
    }

    validateForm() {
        this.setState({formValid: this.state.nameValid &&
            this.state.emailValid &&
            this.state.phoneValid &&
            this.state.loginValid &&
            this.state.passwordValid &&
            this.state.passwordConfirmValid
        });
    }

    render(){
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const signUpStyle = {
            maxWidth: '256px'
        }
        
        return(
            <>
            {errorBaner}
            <Form style={signUpStyle}>
                <h2>Регистрация</h2>
                <FormGroup>
                    <Label for="name">Имя пользователя</Label>
                    <Input invalid={!this.state.nameValid} required type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.name}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="email">Email</Label>
                    <Input invalid={!this.state.emailValid} type="email" name="email" id="email" value={this.state.email} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.email}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="phone">Телефон</Label>
                    <Input invalid={!this.state.phoneValid} type="tel" name="phone" id="phone" value={this.state.phone} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.phone}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="login">Логин</Label>
                    <Input invalid={!this.state.loginValid} required type="text" name="login" id="login" value={this.state.login} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.login}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="password">Пароль</Label>
                    <Input invalid={!this.state.passwordValid} required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.password}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="passwordConfirm">Повторите пароль</Label>
                    <Input invalid={!this.state.passwordConfirmValid} required type="password" name="passwordConfirm" id="passwordConfirm" value={this.state.passwordConfirm} onChange={this.handleInputChange}/>
                    <FormFeedback invalid>{this.state.formErrors.passwordConfirm}</FormFeedback>
                </FormGroup>
                <Button color="primary" disabled = {!this.state.formValid} onClick={() => this.register()}>Зарегистрироваться</Button>
            </Form>
            </>
        )
    }

    register(){
        if (!this.state.formValid)
        {
            this.setState({errorMessage: 'Форма не валидна'})
            return;
        }
        var data = {
            userName: this.state.name,
            contactEmail: this.state.email, 
            contactPhone: this.state.phone, 
            login: this.state.login, 
            password: this.state.password, 
            passwordConfirm: this.state.passwordConfirm
        };

        fetch('api/Users', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            debugger;
            if (response.ok){
                this.props.history.push("/signIn");
            }
            else {
                return response.json();
            }
        }).then((data) => {
            this.setState({errorMessage: data.message});
        }).catch((ex) => {
            this.setState({errorMessage: ex.toString()});
        });
    }
}