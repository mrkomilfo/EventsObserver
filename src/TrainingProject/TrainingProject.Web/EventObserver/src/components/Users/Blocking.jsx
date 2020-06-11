import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/AuthHelper.js';

export default class Blocking extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,
            id: null,
            userName: '',
            isBanned: false,
            days: 0, hours: 0,
            formErrors: { days: 0, hours: 0 },
            formValid: true, daysValid: true, hoursValid: true,
            errorMessage: '', error: false
            
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.cancel = this.cancel.bind(this);
        this.ban = this.ban.bind(this);
        this.unban = this.unban.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadUser(parsed['id']);
        }
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

        let daysValid = this.state.daysValid;
        let hoursValid = this.state.hoursValid;

        switch(fieldName){
            case 'days':
                daysValid = value.match(/^((0|([1-9][0-9]{0,9})))$/i)
                fieldValidationErrors.days = daysValid ? '' : 'Количество дней указано неверно';
                break;
            case 'hours':
                hoursValid = value.match(/^((0|([1-9][0-9]{0,9})))$/i)
                fieldValidationErrors.hours = hoursValid ? '' : 'Количество часов указано неверно';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            daysValid: daysValid,
            hoursValid: hoursValid
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
            this.state.daysValid &&
            this.state.hoursValid 
        });
    }

    cancel()
    {
        this.props.history.push(`/user?id=${this.state.id}`);
    }

    renderContent(){
        return(
            <>
            <h2>Блокировка пользователя {this.state.userName}</h2>
            <Form>
                <FormGroup>
                    <Label for="days">Количество дней</Label>
                    <Input invalid={!this.state.daysValid} required type="number" name="days" id="days" value={this.state.days} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.days}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="hours">Количество часов</Label>
                    <Input invalid={!this.state.hoursValid} required type="number" name="hours" id="hours" value={this.state.hours} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.hours}</FormFeedback>
                </FormGroup>
                <div>
                    {this.state.isBanned ? <Button color="warning" onClick={() => this.unban()}>Разблокировать</Button> : null}{' '}
                    <Button disabled = {!this.state.formValid} color="danger" onClick={() => this.ban()}>Заблокировать</Button>{' '}
                    <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                </div>
            </Form>
            </>
        )
    }

    render(){
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderContent();

        return(
            <>
            {errorBaner}
            {content}
            </>
        )
    }

    async loadUser(userId) {
        const token = await AuthHelper.getToken();
        fetch(`api/Users/${userId}/ban`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            },
        })
        .then((response) => {
            if (!response.ok) {
                this.setState({
                    error: true
                });
            }
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data.message,
                });
            }
            else {
                this.setState({ 
                    id: data.id,
                    userName: data.userName,
                    isBanned: data.isBanned,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    async ban(){
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let data = {
            id: this.state.id,
            days: parseInt(this.state.days, 10),
            hours: parseInt(this.state.hours, 10),
        }
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch('api/Users/ban', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
                'Authorization': 'Bearer ' + token     
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.id}`);
            }
            else {
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            debugger;
            if(this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    async unban(){
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch(`api/Users/${this.state.id}/unban`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.id}`);
            }
            else {
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            if(this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}
