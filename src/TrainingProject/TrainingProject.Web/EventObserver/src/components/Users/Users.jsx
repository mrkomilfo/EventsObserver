import React, { Component } from 'react';
import { Alert, Table, Button, Input, InputGroup, InputGroupAddon } from 'reactstrap';
import { Link } from 'react-router-dom';
import UsersPaginator from './UsersPaginator';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper.js';

export default class Users extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,
            query: window.location.search, 
            users: [], 
            currentPage: 0, 
            pageSize: 20, 
            totalRecords: 0,  
            userName: '',
            error: false, 
            errorMessage: '' 
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

    getQuerryTrailer(){
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

    renderUsersList(users){
        return(
            <>
            <p>{`Найдено пользователей: ${this.state.totalRecords}`}</p>
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
            <UsersPaginator currentPage={this.state.currentPage} totalPages={Math.ceil(this.state.totalRecords / this.state.pageSize)}/>        
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
            : this.renderUsersList(this.state.users);

        const headerStyle = {
            marginBottom: '8px',
            display: 'flex',
            justifyContent: 'space-between'
        }

        return (
            <>
            {errorBaner}
            <div style={headerStyle}>
                <h2>Пользователи</h2>
                <div>
                    <InputGroup>
                        <Input type="text" name="userName" id="userName" value={this.state.userName} placeholder="Имя пользователя" onChange={this.handleInputChange} />
                        <InputGroupAddon addonType="append"><Button color="primary" tag={Link} to={`/users${this.getQuerryTrailer()}`}>Поиск</Button></InputGroupAddon>
                    </InputGroup>
                </div>
            </div>
            {content}
            </>
        );
    }

    async loadUsers(){
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
        const token = await AuthHelper.getToken();
        fetch(`api/Users${queryTrailer}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            },
        })
        .then((response) => {
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({errorMessage: data});
            }
            else {
                this.setState({ 
                    users: data.records, 
                    currentPage: data.currentPage, 
                    pageSize: data.pageSize, 
                    totalRecords: data.totalRecords, 
                    loading: false  
                });
            }
        }).catch((ex) => {
            this.setState({errorMessage: ex.toString()});
        });
    }
}