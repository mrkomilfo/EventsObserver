import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/AuthHelper'

export default class EditEvent extends Component{
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false,
            errorMessage: '',

            userId: null,
            userName: '',
            roleId: null,
        
            roles: [],
        };
        this.changeRole = this.changeRole.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.cancel = this.cancel.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadUser(parsed['id']);
        }
        this.loadRoles();
    }

    handleChange(event) {
        this.setState({ roleId: event.target.value });
    }

    cancel()
    {
        this.props.history.push(`/user?id=${this.state.userId}`);
    }

    renderContent(){
        const formStyle = {
            maxWidth: '256px'
        }
        const rolesSelect = this.state.roles.map(r => <option key={r.id.toString()} value={r.id}>{r.name}</option>)
        return(
            <>
            <h2>Изменение роли пользователя {this.state.userName}</h2>
            <Form style={formStyle}>
                <FormGroup>
                    <Label for="role">Роль</Label>
                    <Input type="select" name="role" id="role" value={this.state.roleId} onChange={this.handleChange}>
                        {rolesSelect}
                    </Input>
                </FormGroup>
                <div>
                    <Button color="primary" onClick={() => this.changeRole()}>Сохранить</Button>{' '}
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

    loadRoles() {
        const token = AuthHelper.getToken();
        fetch('api/Users/roles', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        })
        .then((response) => {
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({
                    errorMessage: data.message
                });
            }
            else {
                this.setState({ 
                    roles: data 
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    async loadUser(userId) {
        const token = await AuthHelper.getToken();
        fetch(`api/Users/${userId}/role`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data.message 
                });
            }
            else {
                this.setState({ 
                    userId: data.userId,
                    userName: data.userName,
                    roleId: data.roleId,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    changeRole()
    {
        let data = {
            userId: this.state.userId,
            roleId: parseInt(this.state.roleId, 10),
        }
        const token = AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch('api/Users/role', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.userId}`);
            }
            else {
                this.setState({
                    error: true
                });
                return response.json();
            }
        }).then((data) => {
            if (this.state.error)
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