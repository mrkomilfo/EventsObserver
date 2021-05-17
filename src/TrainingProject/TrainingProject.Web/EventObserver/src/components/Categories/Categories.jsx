import React, { Component } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Link } from 'react-router-dom';

import AuthHelper from '../../Utils/authHelper.js';

export default class Categories extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true, 
            error: false,
            deleteModal: false,
            categoryToDeleteId: null,
            categories: []
        };

        this.deleteCategory = this.deleteCategory.bind(this);
        this.toggleDeleteModal = this.toggleDeleteModal.bind(this);
        this.onDeleteButtonClick = this.onDeleteButtonClick.bind(this);
    }

    componentDidMount() {
        this.loadCategories();
    }

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    onDeleteButtonClick(e) {
        this.setState({categoryToDeleteId: parseInt(e.target.dataset.id, 10)}, this.toggleDeleteModal);
    }

    renderCategoriesList() {
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
            <div className="list-group mx-auto" style={{maxWidth: '900px'}}>
                <div class="list-group-item bg-light d-flex justify-content-between">
                    <h3 className="m-0">Категории</h3>
                    <div>
                        <Button color="primary" tag={Link} to={"/newCategory"}>
                            Добавить категорию
                        </Button>
                    </div>
                </div>
                <div className="list-group-item">
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Название</th>
                                <th>Описание</th>
                                <th style={{width: '118px'}}>Действие</th>
                            </tr>
                        </thead>
                        <tbody>
                            {this.state.categories.map((c, index) => 
                                <tr key={c.id.toString()}>
                                    <th scope="row">{index+1}</th>
                                    <td>{c.name}</td>
                                    <td>{c.description}</td>
                                    <td>
                                        <Link type="button" to={`editCategory?id=${c.id}`} className="btn btn-outline-primary mr-2">
                                            <i class="bi bi-pencil"></i>
                                        </Link>
                                        <Button outline color="danger" data-id={c.id} onClick={e => this.onDeleteButtonClick(e)}>
                                            <i class="icon bi-trash" data-id={c.id}></i>
                                        </Button>
                                    </td>
                                </tr>)}
                        </tbody>
                    </table>
                </div>
                {deleteModal}
            </div>
        )
    }

    render() {
        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCategoriesList(this.state.categories);

        return (
            <>
                {content}
            </>
        );
    }

    async loadCategories() {
        fetch('api/categories', {
            method: 'GET',
        })
        .then((response) => {
            this.setState({
                error: !response.ok
            });
            return response.json();
        }).then((data) => {
            if (this.state.error){
                console.log(data);
            }
            else {
                this.setState({ 
                    categories: data
                });
            }
        }).catch((ex) => {
            console.log(ex.toString());
        }).finally(() => {
            this.setState({ 
                loading: false 
            });
        })
    }

    async deleteCategory() {
        AuthHelper.fetchWithCredentials('api/categories/' + this.state.categoryToDeleteId, {
            method: 'DELETE',
        }).then((response) => {
            if (response.ok) {
                this.setState({categories: this.state.categories.filter(c => c.id !== this.state.categoryToDeleteId)},
                    this.toggleDeleteModal)
            } 
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                this.setState({
                    error: true
                });
                return response.json()
            }
        }).then((data) => {
            if (this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}