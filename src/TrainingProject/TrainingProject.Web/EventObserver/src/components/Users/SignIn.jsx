import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper'

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { login: '', password: '', errorMessage: '', error: false };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.logIn = this.logIn.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;

        this.setState({
          [name]: value
        });
    }

    render(){
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const signInStyle = {
            maxWidth: '256px'
        }

        return( 
            <>
            {errorBaner}
            <Form style={signInStyle}>    
                <h2>Вход</h2>            
                <FormGroup>
                    <Label for="login">Логин</Label>
                    <Input required type="text" name="login" id="login" value={this.state.login} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="password">Пароль</Label>
                    <Input required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange}/>
                </FormGroup>
                <Button color="primary" onClick={() => this.logIn()}>Войти</Button>
            </Form>
            </>
        )
    }

    logIn() {
        var data = {
            login: this.state.login,
            password: this.state.password
        };
        fetch('api/Users/signIn', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
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
                AuthHelper.saveAuth(data.name, data.accessToken, data.role);
                this.props.history.push("/events");
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}