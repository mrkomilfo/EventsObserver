import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

export default class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor (props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
            role: AuthHelper.getRole()
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ 
            role: AuthHelper.getRole() 
        });
    }

    toggleNavbar () {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    logout(){
        AuthHelper.clearAuth();
        this.setState({
            role: 'Guest'
        });
    }

    render () {
        let Events = 
            <NavItem>
                <NavLink tag={Link} to="/">События</NavLink>
            </NavItem>;

        let NewEvent = this.state.role != 'Guest' ?
            <NavItem>
                <NavLink tag={Link} to="/newEvent">Добавить событие</NavLink>
            </NavItem> : null;

        let Users = (this.state.role == 'Admin' || this.state.role == 'AccountManager') ?
            <NavItem>
                <NavLink tag={Link} to="/users">Пользователи</NavLink>
            </NavItem> : null;

        let Categories = this.state.role == 'Admin' ?
            <NavItem>
                <NavLink tag={Link} to="/categories">Категории</NavLink>
            </NavItem> : null;

        let Identity = this.state.role == 'Guest' ?
            <>
                <NavItem>
                    <NavLink tag={Link} to="/signUp">Регистрация</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} to="/signIn">Вход</NavLink>
                </NavItem>
            </> :
            <NavItem>
                <NavLink tag={Link} to="/signIn" onClick={()=>this.logout()}>Выход</NavLink>
            </NavItem>;

        return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
            <Container>
                <NavbarBrand tag={Link} to="/">EventsObserver</NavbarBrand>
                <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                <ul className="navbar-nav flex-grow">
                    {Events}
                    {NewEvent}
                    {Users}
                    {Categories}
                    {Identity}
                </ul>
                </Collapse>
            </Container>
            </Navbar>
        </header>
        );
    }
}