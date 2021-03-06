import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper'

export default class NewCategory extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            name: '', 
            description: '',  
            formErrors: { 
                name: '',  
                description: ''
            },
            formValid: false,
            nameValid: false, 
            descriptionValid: true,

            error: false,
            errorMessage: '',
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.createCategory = this.createCategory.bind(this);
        this.cancel = this.cancel.bind(this);
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

        switch(fieldName){
            case 'name':
                nameValid = !!value;
                fieldValidationErrors.name = nameValid ? '' : 'У мероприятия должно быть название';
                break;
            case 'description':
                descriptionValid = value.length <= 127;
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
        this.props.history.push(`/categories`);
    }

    render() {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        return(
            <div className="mx-auto" style={{maxWidth: '720px'}}>
                {errorBaner}
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Новая категория</h3>
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
                                <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.createCategory()}>Добавить</Button>{' '}
                                <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                            </div>
                        </Form>
                    </div>
                </div>
            </div>
        )
    }

    async createCategory() {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let data = {
            name: this.state.name,
            description: this.state.description
        }
        AuthHelper.fetchWithCredentials('api/categories', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.history.push("/categories");
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