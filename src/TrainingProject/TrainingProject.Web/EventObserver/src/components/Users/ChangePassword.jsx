import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper.js';

export default class ChangePassword extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            id: AuthHelper.getId(), 
            oldPassword: '', 
            newPassword: '', 
            newPasswordConfirm: '',
            formErrors: { 
                oldPassword: '', 
                newPassword: '', 
                newPasswordConfirm: ''
            },
            formValid: false, 
            oldPasswordValid: false, 
            newPasswordValid: false, 
            newPasswordConfirmValid: false,
            error: false, 
            errorMessage: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.changePassword = this.changePassword.bind(this);
        this.cancel = this.cancel.bind(this);
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

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;

        let oldPasswordValid = this.state.oldPasswordValid;
        let newPasswordValid = this.state.newPasswordValid;
        let newPasswordConfirmValid = this.state.newPasswordConfirmValid;

        switch(fieldName){
            case 'oldPassword':
                oldPasswordValid = value.length >= 0;
                fieldValidationErrors.oldPassword = oldPasswordValid ? '' : 'Введите старый пароль';
                break;
            case 'newPassword':
                newPasswordValid = value.length >= 6;
                fieldValidationErrors.newPassword = newPasswordValid ? '' : 'Минимальная длина - 6';
                break;
            case 'newPasswordConfirm':
                newPasswordConfirmValid = value === this.state.newPassword;
                fieldValidationErrors.newPasswordConfirm = newPasswordConfirmValid ? '' : 'Пароли не совпадают';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            oldPasswordValid,
            newPasswordValid,
            newPasswordConfirmValid
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.oldPasswordValid &&
                this.state.newPasswordValid &&
                this.state.newPasswordConfirmValid
        });
    }

    cancel() {
        this.props.history.push(`/user?id=${this.state.id}`);
    }

    render() {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const formStyle = {
            maxWidth: '272px'
        }
        
        return(
            <>
                {errorBaner}
                <h2>Изменение пароля</h2>
                <Form style={formStyle}>
                    <FormGroup>
                        <Label for="oldPassword">Старый пароль</Label>
                        <Input invalid={!this.state.oldPasswordValid} required type="password" name="oldPassword" id="oldPassword" value={this.state.oldPassword} onChange={this.handleInputChange}/>
                        <FormFeedback>{this.state.formErrors.oldPassword}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="newPassword">Новый пароль</Label>
                        <Input invalid={!this.state.newPasswordValid} required type="password" name="newPassword" id="newPassword" value={this.state.newPassword} onChange={this.handleInputChange}/>
                        <FormFeedback>{this.state.formErrors.newPassword}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="newPasswordConfirm">Повторите новый пароль</Label>
                        <Input invalid={!this.state.newPasswordConfirmValid} required type="password" name="newPasswordConfirm" id="newPasswordConfirm" value={this.state.newPasswordConfirm} onChange={this.handleInputChange}/>
                        <FormFeedback>{this.state.formErrors.newPasswordConfirm}</FormFeedback>
                    </FormGroup>
                    <div>
                        <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.changePassword()}>Сохранить</Button>{' '}
                        <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                    </div>
                </Form>
            </>
        )
    }

    async changePassword() {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        var data = {
            id: this.state.id,
            oldPassword: this.state.oldPassword,
            newPassword: this.state.newPassword, 
            newPasswordConfirm: this.state.newPasswordConfirm
        };

        AuthHelper.fetchWithCredentials('api/Users/changePassword', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                AuthHelper.clearAuth();
                this.props.history.push("/signIn");
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
            if(this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}