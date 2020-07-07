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
            noContent: false,
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
        if (parsed && parsed['id']) {
            this.loadData(parsed['id']);
        }
        else {
            this.props.history.push("/404")
        }
    }

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    renderCategory() {
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

    render() {
        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.noContent
                ? <Alert color="info">
                    {"Категория удалена или ещё не создана"}
                  </Alert> 
                : this.renderCategory()
            );

        return(
            <>
                <h2>Информация о категории</h2>
                {content}
            </>
        )
    }

    async loadData(categoryId) {
        AuthHelper.fetchWithCredentials('api/Categories/' + categoryId)
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
                if (this.state.error){
                    console.log(data);
                }
                else {
                    this.setState({ 
                        id: data.id,
                        name: data.name,
                        description: data.description,
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

    async deleteCategory() {
        AuthHelper.fetchWithCredentials('api/Categories/' + this.state.id, {
            method: 'DELETE',
        }).then((response) => {
            if (response.ok) {
                this.props.history.push("/categories");                   
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