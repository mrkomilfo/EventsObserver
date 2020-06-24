import React, { Component } from 'react';
import queryString from 'query-string';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper'

export default class EditCategory extends Component {
    constructor(props) {
        super(props);
        this.state = {     
            loading: true,

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
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.editCategory = this.editCategory.bind(this);
        this.cancel = this.cancel.bind(this);
    }
    
    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadCategory(parsed['id']);
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

    validateField(fieldName, value){
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
            nameValid: nameValid,
            descriptionValid: descriptionValid,
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.nameValid &&
                this.state.descriptionValid
        });
    }

    cancel()
    {
        this.props.history.push(`/category?id=${this.state.id}`);
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
                <h2>Редактирование категории</h2>
                {content}
            </>
        )
    }

    renderCategory()
    {
        return(
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
        )
    }

    async loadCategory(categoryId) {
        AuthHelper.fetchWithCredentials('api/Categories/' + categoryId)
            .then((response) => {
            if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data 
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

    async editCategory(){
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
        AuthHelper.fetchWithCredentials('api/Categories', {
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
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            if(this.state.error)
            {
                this.setState({
                    errorMessage: data
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}