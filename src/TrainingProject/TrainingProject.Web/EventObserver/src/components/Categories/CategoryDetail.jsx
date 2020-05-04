import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/authHelper.js';

export default class CategoryDetail extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,
            error: false,
            errorMessage: '',
            id: null,
            name: '',
            description: '',
            deleteModal: false,
        }
        this.deleteCategory = this.deleteCategory.bind(this);
        this.toggleDeleteModal = this.toggleDeleteModal.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadData(parsed['id']);
        }
    }

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    renderCategory(){
        const deleteModal = 
            <Modal isOpen={this.state.deleteModal} toggle={this.toggleDeleteModal}>
                <ModalHeader toggle={this.toggleDeleteModal}>Подтвердите действие</ModalHeader>
                <ModalBody>
                    Вы действительно хотите удалить данную категорию?
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.deleteCategory}>Да</Button>{' '}
                    <Button color="secondary" onClick={this.toggleDeleteModal}>Отмена</Button>
                </ModalFooter>
            </Modal>

        return(
            <>
                <h4>Название:</h4>
                <p>{this.state.name}</p>
                <h4>Описание:</h4>
                <p>{this.state.description || 'Отсутствует'}</p>
                <div>
                    <Button outline color="primary" tag={Link} to={`/editCategory?id=${this.state.id}`}>Редактировать</Button>{' '}
                    <Button color="danger" onClick={this.toggleDeleteModal}>Удалить</Button>
                </div>
                {deleteModal}
            </>
        )
    }

    render()
    {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCategory();

        return(
            <>
                {errorBaner}
                <h2>Информация о категории</h2>
                {content}
            </>
        )
    }

    async loadData(categoryId) {
        const token = AuthHelper.getToken();
        fetch('api/Categories/' + categoryId, {
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
                    id: data.id,
                    name: data.name,
                    description: data.description,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    deleteCategory() {
        const token = AuthHelper.getToken();
        fetch('api/Categories/' + this.state.id, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            if (response.ok) {
                this.props.history.push("/categories");                   
            } 
            else {
                this.setState({error: true})
                return response.json()
            }
        }).then((data) => {
            if (this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                })
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            })
        });
    }
}