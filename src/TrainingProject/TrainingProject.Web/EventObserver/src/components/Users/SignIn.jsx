import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper';
import './SignIn.css';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            login: '', 
            password: '', 
            errorMessage: '', 
            error: false 
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.logIn = this.logIn.bind(this);
        this.enterPressed = this.enterPressed.bind(this);
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
            this.logIn();
        }
    };

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
            <h2>Вход</h2>  
            <Form style={signInStyle}>    
                <FormGroup>
                    <Label for="login">Логин</Label>
                        <Input required type="text" name="login" id="login" value={this.state.login} onChange={this.handleInputChange} onKeyPress={this.enterPressed}/>
                </FormGroup>
                <FormGroup>
                    <Label for="password">Пароль</Label>
                        <Input required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange} onKeyPress={this.enterPressed}/>
                    </FormGroup>
                    <div>
                        <Button color="primary" onClick={this.logIn}>Войти</Button>{' '}
                        <Button className="googleButton" outline color="primary"></Button>{' '}
                    </div>
            </Form>
            </>
        )
    }

    logIn() {
        let auth_data = {
            login: this.state.login,
            password: this.state.password
        };
        fetch('api/Users/signIn', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(auth_data)
        }).then((response) => {
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({
                    errorMessage: data
                });
            }
            else {
                AuthHelper.saveAuth(data.name, data.role, data.accessToken, data.refreshToken);
                this.props.history.goBack();
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}