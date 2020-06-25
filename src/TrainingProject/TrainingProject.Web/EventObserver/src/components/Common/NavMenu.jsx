import React, { Component } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, 
    Modal, ModalHeader, ModalBody, ModalFooter, Button } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

import './NavMenu.css';

export default class NavMenu extends Component {
    static displayName = NavMenu.name;

    constructor (props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.toggleLogoutModal = this.toggleLogoutModal.bind(this);
        this.logout = this.logout.bind(this);

        this.state = {
            collapsed: true,
            logoutModal: false,
            role: AuthHelper.getRole(),
            id: AuthHelper.getId()
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ 
            role: AuthHelper.getRole(),
            id: AuthHelper.getId()
        });
    }

    toggleNavbar () {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }

    toggleLogoutModal(){
        this.setState({
            logoutModal: !this.state.logoutModal
        });
    }

    render () {
        const events = 
            <NavItem>
                <NavLink tag={Link} to="/">События</NavLink>
            </NavItem>;

        const newEvent = this.state.role !== 'Guest' ?
            <NavItem>
                <NavLink tag={Link} to="/newEvent">Добавить событие</NavLink>
            </NavItem> : null;

        const users = (this.state.role === 'Admin' || this.state.role === 'Account manager') ?
            <NavItem>
                <NavLink tag={Link} to="/users">Пользователи</NavLink>
            </NavItem> : null;

        const categories = this.state.role === 'Admin' ?
            <NavItem>
                <NavLink tag={Link} to="/categories">Категории</NavLink>
            </NavItem> : null;

        const identity = this.state.role === 'Guest' ?
            <>
                <NavItem>
                    <NavLink tag={Link} to="/signUp">Регистрация</NavLink>
                </NavItem>
                <NavItem>
                    <NavLink tag={Link} to="/signIn">Вход</NavLink>
                </NavItem>
            </> :
            <>
                <NavItem>
                    <NavLink tag={Link} to={`/user?id=${this.state.id}`}>Мой профиль</NavLink>
                </NavItem>
                <NavItem>  
                    <NavLink style={{cursor: 'pointer'}} onClick={this.toggleLogoutModal}>Выход</NavLink>
                </NavItem>
            </>

        const logoutModal = 
            <Modal isOpen={this.state.logoutModal} toggle={this.toggleLogoutModal}>
                <ModalHeader toggle={this.toggleLogoutModal}>Подтвердите действие</ModalHeader>
                <ModalBody>
                    Вы действительно хотите выйти из аккаунта?
                </ModalBody>
                <ModalFooter>
                <Button tag={Link} to="/signIn" color="primary" onClick={this.logout}>Подтвердить</Button>{' '}
                <Button color="secondary" onClick={this.toggleLogoutModal}>Отмена</Button>
                </ModalFooter>
            </Modal>

        return (
            <header id="header">
                <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                <Container>
                    <NavbarBrand tag={Link} to="/">EventObserver</NavbarBrand>
                    <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                    <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                    <ul className="navbar-nav flex-grow">
                        {events}
                        {newEvent}
                        {users}
                        {categories}
                        {identity}
                        {logoutModal}
                    </ul>
                    </Collapse>
                </Container>
                </Navbar>
            </header>
        );
    }

    async logout() {
        AuthHelper.clearAuth();
        this.setState({
            role: 'Guest',
            logoutModal: false
        });

        AuthHelper.fetchWithCredentials(`api/Users/${this.state.id}/logout`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        });
    }
}