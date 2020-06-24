import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper'

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

    async loadRoles() {
        AuthHelper.fetchWithCredentials('api/Users/roles')
            .then((response) => {
                if (response.status === 401) {
                    this.props.history.push("/signIn");
                }
                else {
                    this.setState({ error: !response.ok });
                    return response.json();
                }
            }).then((data) => {
                if (this.state.error){
                    this.setState({
                        errorMessage: data
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
        AuthHelper.fetchWithCredentials(`api/Users/${userId}/role`)
            .then((response) => {
                if (response.status === 401) {
                    this.props.history.push("/signIn");
                }
                else {
                    this.setState({ error: !response.ok });
                    return response.json();
                }
            }).then((data) => {
                if (this.state.error){
                    this.setState({ 
                        errorMessage: data 
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

    async changeRole()
    {
        let data = {
            userId: this.state.userId,
            roleId: parseInt(this.state.roleId, 10),
        }

        AuthHelper.fetchWithCredentials('api/Users/role', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.userId}`);
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
            if (this.state.error)
            {
                this.setState({
                    errorMessage: data
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}