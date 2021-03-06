import React, { Component } from 'react';
import queryString from 'query-string';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';

import AuthHelper from '../../Utils/authHelper';
import ErrorPage from '../Common/ErrorPage';

export default class EditCategory extends Component {
    constructor(props) {
        super(props);
        this.state = {     
            loading: true,
            statusCode: 200,

            id: null,
            name: '', 
            description: '',  
            formErrors: { 
                name: '',  
                desription: ''
            },
            formValid: true,
            nameValid: true, 
            descriptionValid: true,

            error: false,
            errorMessage: '',

            noContent: false
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.editCategory = this.editCategory.bind(this);
        this.cancel = this.cancel.bind(this);
    }
    
    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed && parsed['id']) {
            this.loadCategory(parsed['id']);
        }
        else {
            this.setState({
                statusCode: 404
            });
        }
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        const value = target.value;

        this.setState({
            [name]: value
        }, 
            () => { this.validateField(name, value) 
        });   
    }

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;

        let nameValid = this.state.nameValid;
        let descriptionValid = this.state.descriptionValid;

        switch(fieldName) {
            case 'name':
                nameValid = !!value;
                fieldValidationErrors.name = nameValid ? '' : 'У мероприятия должно быть название';
                break;
            case 'description':
                descriptionValid = value.length <= 256;
                fieldValidationErrors.description = descriptionValid ? '' : 'Описание слишком длинное';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            nameValid,
            descriptionValid,
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.nameValid &&
                this.state.descriptionValid
        });
    }

    cancel() {
        this.props.history.push(`/category?id=${this.state.id}`);
    }

    renderCategory() {
        const errorBaner = this.state.errorMessage ? 
            <Alert color="danger">
                {this.state.errorMessage}
            </Alert> : null;

        return(
            <div className="mx-auto" style={{maxWidth: '720px'}}>
                {errorBaner}
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Редактирование категории</h3>
                    </div>
                    <div className="list-group-item">
                        <Form>
                            <FormGroup>
                                <Label for="name">Название</Label>
                                <Input invalid={!this.state.nameValid} required type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.name}</FormFeedback>
                            </FormGroup>
                            <FormGroup>
                                <Label for="description">Описание</Label>
                                <Input invalid={!this.state.descriptionValid} required type="textarea" name="description" id="description" value={this.state.description} onChange={this.handleInputChange}/>
                                <FormFeedback>{this.state.formErrors.description}</FormFeedback>
                            </FormGroup>
                            <div>
                                <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.editCategory()}>Сохранить</Button>{' '}
                                <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                            </div>
                        </Form>
                    </div>
                </div>
            </div>
        )
    }

    render() {
        let content;
        switch (this.state.statusCode) {
            case 204:
                content = 
                    <Alert color="info">
                        {"Категория удалена или ещё не создана"}
                    </Alert>
                break;
            case 403:
            case 404:
            case 500:
                content = <ErrorPage code={this.state.statusCode}/>
                break;
            default:
                content = 
                this.state.loading 
                    ? <p><em>Loading...</em></p> 
                    : this.renderCategory()
        }

        return(
            <>
                {content}
            </>
        )
    }

    async loadCategory(categoryId) {
        AuthHelper.fetchWithCredentials('api/categories/' + categoryId)
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

    async editCategory() {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let data = {
            id: this.state.id,
            name: this.state.name,
            description: this.state.description
        }
        AuthHelper.fetchWithCredentials('api/categories', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/category?id=${this.state.id}`);
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