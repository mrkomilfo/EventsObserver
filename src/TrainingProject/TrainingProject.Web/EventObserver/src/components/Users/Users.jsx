import React, { Component } from 'react';
import { Table, Button, Input, InputGroup, InputGroupAddon } from 'reactstrap';
import { Link } from 'react-router-dom';
import UsersPaginator from './UsersPaginator';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper.js';

export default class Users extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false,

            query: window.location.search, 
            users: [], 
            currentPage: 0, 
            pageSize: 20, 
            totalRecords: 0,  
            userName: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    componentDidMount() {
        this.loadUsers();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.query !== window.location.search) {
            this.setState({ query: window.location.search });
            this.loadUsers();
        }
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;
        
        this.setState({
          [name]: value
        });
    }

    getQueryTrailer() {
        let isFirst = true
        let queryTrailer = '';
        if (this.state.userName)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `search=${this.state.userName}`;
            isFirst = false;
        }
        return queryTrailer;
    }

    render() {
        return(
            this.state.loading ? <p><em>Loading...</em></p> :
            <div className="list-group mx-auto" style={{maxWidth: '900px'}}>
                <div className="list-group-item bg-light d-flex justify-content-between">
                    <h3 className="m-0">Пользователи</h3>
                    <div>
                        <InputGroup>
                            <Input type="text" name="userName" id="userName" value={this.state.userName} placeholder="Имя пользователя" onChange={this.handleInputChange} />
                            <InputGroupAddon addonType="append"><Button color="primary" tag={Link} to={`/users${this.getQueryTrailer()}`}>Поиск</Button></InputGroupAddon>
                        </InputGroup>
                    </div>
                </div>
                <div className="list-group-item">
                    <Table striped>
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Имя пользователя</th>
                                <th>Роль</th>
                                <th>Статус</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.users.map((u, index) => 
                            <tr key={u.id}>
                                <th scope="row">{this.state.currentPage*this.state.pageSize+index+1}</th>
                                <td><Link to={`user?id=${u.id}`}>{u.userName}</Link></td>
                                <td>{u.role}</td>
                                <td>{u.status || 'без ограничений'}</td>
                            </tr>)}
                        </tbody>
                    </Table>  
                </div>
                <div className="list-group-item pb-0">
                    <UsersPaginator currentPage={this.state.currentPage} totalPages={Math.ceil(this.state.totalRecords / this.state.pageSize)}/>        
                </div>
            </div>
        );
    }

    async loadUsers() {
        let page; let search;
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            page = parsed['page'] || 0;
            search = parsed['search']; 
        }

        let queryTrailer = '?page=' + page;
        if (search) {
            queryTrailer += `&search=${search}`
        }
        AuthHelper.fetchWithCredentials(`api/users${queryTrailer}`)
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
                        users: data.records, 
                        currentPage: data.currentPage, 
                        pageSize: data.pageSize, 
                        totalRecords: data.totalRecords
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
}