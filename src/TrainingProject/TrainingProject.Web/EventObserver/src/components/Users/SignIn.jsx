import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input } from 'reactstrap';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { login: '', password: '' };
        this.handleInputChange = this.handleInputChange.bind(this);
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
        const signInStyle = {
            maxWidth: '256px'
        }

        return( 
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
                <Button color="primary">Войти</Button>
            </Form>
        )
    }
}