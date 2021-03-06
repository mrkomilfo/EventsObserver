import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Alert } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper'

export default class ChangeRole extends Component{
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false,
            noContent: false,

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
        this.setState({ 
            roleId: event.target.value 
        });
    }

    cancel() {
        this.props.history.push(`/user?id=${this.state.userId}`);
    }

    renderContent() {
        const rolesSelect = this.state.roles.map(r => <option key={r.id.toString()} value={r.id}>{r.name}</option>)

        return(
            <div className="mx-auto" style={{width: '420px'}}>
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Изменение роли пользователя {this.state.userName}</h3>
                    </div>
                    <div className="list-group-item">
                        <Form>
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
                    </div>
                </div>
            </div>
        )
    }

    render() {
        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.noContent
                ? <Alert color="info">
                    {"Пользователь удалён или ещё не зарегистрирован"}
                </Alert>
                : this.renderContent()
            );

        return(
            <>
                {content}
            </>
        )
    }

    async loadRoles() {
        AuthHelper.fetchWithCredentials('api/users/roles')
            .then((response) => {
                if (response.status === 401) {
                    this.props.history.push("/signIn");
                }
                else {
                    this.setState({ 
                        error: !response.ok 
                    });
                    return response.json();
                }
            }).then((data) => {
                if (this.state.error){
                    console.log(data);
                }
                else {
                    this.setState({ 
                        roles: data 
                    });
                }
            }).catch((ex) => {
                console.log(ex.toString());
            });
    }

    async loadUser(userId) {
        AuthHelper.fetchWithCredentials(`api/users/${userId}/role`)
            .then((response) => {
                if (response.status === 401) {
                    this.props.history.push("/signIn");
                }
                else {
                    this.setState({
                        error: !response.ok,
                        noContent: response.status === 204
                    });
                    return response.json();
                }
            }).then((data) => {
                if (this.state.error) {
                    console.log(data);
                }
                else {
                    this.setState({ 
                        userId: data.userId,
                        userName: data.userName,
                        roleId: data.roleId,
                    });
                }
            }).catch((ex) => {
                console.log(ex.toString());
            }).finally(() => {
                this.setState({
                    loading: false
                });
            });
    }

    async changeRole()
    {
        let data = {
            userId: this.state.userId,
            roleId: parseInt(this.state.roleId, 10),
        }

        AuthHelper.fetchWithCredentials('api/users/role', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok) {
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
            if(this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}