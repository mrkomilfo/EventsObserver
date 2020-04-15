import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input } from 'reactstrap';

export default class SignUp extends Component {
    constructor(props) {
        super(props);
        this.state = { name: '', email: '', phone: '', login: '', password: '', passwordConfirm: '' };
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
        const signUpStyle = {
            maxWidth: '256px'
        }
        
        return(
            <Form style={signUpStyle}>
                <h2>Регистрация</h2>
                <FormGroup>
                    <Label for="name">Имя пользователя</Label>
                    <Input required type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="email">Email</Label>
                    <Input type="email" name="email" id="email" value={this.state.email} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="phone">Телефон</Label>
                    <Input type="tel" name="phone" id="phone" value={this.state.phone} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="login">Логин</Label>
                    <Input required type="text" name="login" id="login" value={this.state.login} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="password">Пароль</Label>
                    <Input required type="password" name="password" id="password" value={this.state.password} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup>
                    <Label for="passwordConfirm">Повторите пароль</Label>
                    <Input required type="password" name="passwordConfirm" id="passwordConfirm" value={this.state.passwordConfirm} onChange={this.handleInputChange}/>
                </FormGroup>
                <Button color="primary">Зарегистрироваться</Button>
            </Form>
        )
    }
}