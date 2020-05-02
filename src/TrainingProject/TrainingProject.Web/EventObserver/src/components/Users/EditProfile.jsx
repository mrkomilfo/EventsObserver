import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert, UncontrolledTooltip } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper.js';

export default class EditProfile extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            id: '', userName: '', contactEmail: '', contactPhone: '', hasImage: false, imagePath: '', imageFile: null, fileName: '',
            formErrors: { userName: '', contactEmail: '', contactPhone: ''},
            formValid: true, userNameValid: true, contactEmailValid: true, contactPhoneValid: true,
            error: false, errorMessage: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.editProfile = this.editProfile.bind(this);
        this.cancel = this.cancel.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadUser(parsed['id']);
        }
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value = target.value;

        if (name == 'imageFile')
        {
            this.setState({
                fileName: value,
                imageFile: target.files[0],
                hasImage: true
            }); 
        }
        else{
            this.setState({
                [name]: value
            }, 
                () => { this.validateField(name, value) }
            );
        }
    }

    validateField(fieldName, value){
        let fieldValidationErrors = this.state.formErrors;

        let userNameValid = this.state.userNameValid;
        let contactEmailValid = this.state.contactEmailValid;
        let contactPhoneValid = this.state.contactPhoneValid;

        switch(fieldName){
            case 'userName':
                userNameValid = value.length >= 4;
                fieldValidationErrors.userName = userNameValid ? '' : 'Минимальная длина - 4';
                break;
            case 'contactEmail':
                contactEmailValid = value.match(/^([\w.%+-]+)@([\w-]+\.)+([\w]{2,})$/i) || value.length == 0;
                fieldValidationErrors.contactEmail = contactEmailValid ? '' : 'Неверный формат';
                break;
            case 'contactPhone':
                contactPhoneValid = value.match(/^[\+]?[0-9]{6,12}$/i) || value.length == 0;
                fieldValidationErrors.contactPhone = contactPhoneValid ? '' : 'Неверный формат';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            userNameValid: userNameValid,
            contactEmailValid: contactEmailValid,
            contactPhoneValid: contactPhoneValid,
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
            this.state.userNameValid &&
            this.state.contactEmailValid &&
            this.state.contactPhoneValid
        });
    }

    removeImage()
    {
        this.setState({
            imageFile: null,
            hasImage: false,
            imagePath: '',
            fileName: ''
        })
    }

    cancel()
    {
        this.props.history.push(`/user?id=${this.state.id}`);
    }

    renderProfile(){
        const formStyle = {
            maxWidth: '256px'
        }

        const imageStyle = {
            maxWidth: '420px',
            maxHeight: '420px',
            marginTop: '8px',
        }

        const tipStyle = {
            color: '#70757A',
            textDecoration: 'underline'
        }
        
        let imageBlock;
        if (this.state.imageFile)
        {
            imageBlock = <img style={imageStyle} src={URL.createObjectURL(this.state.imageFile)} alt="profile photo" onClick={(e) => this.removeImage()}/>
        }
        else if (this.state.imagePath)
        {
            imageBlock = <img style={imageStyle} src={this.state.imagePath} alt="profile photo" onClick={(e) => this.removeImage()}/>
        }
        else
        {
            imageBlock = <img style={imageStyle} src='img\users\default.jpg' alt="profile photo" onClick={(e) => this.removeImage()}/>
        }

        return(
            <>           
            <h2>Редактирование профиля</h2>
            <Form style={formStyle}>
                <FormGroup>
                    <Label for="userName">Имя пользователя</Label>
                    <Input invalid={!this.state.userNameValid} required type="text" name="userName" id="userName" value={this.state.userName} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.userName}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="contactEmail">Email</Label>
                    <Input invalid={!this.state.contactEmailValid} type="email" name="contactEmail" id="contactEmail" value={this.state.contactEmail} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.contactEmail}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="contactPhone">Телефон</Label>
                    <Input invalid={!this.state.contactPhoneValid} type="tel" name="contactPhone" id="contactPhone" value={this.state.contactPhone} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.contactPhone}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="imageFile">Фото</Label>{'  '}<span style={tipStyle} id="imageTip">?</span>
                    <UncontrolledTooltip placement="right" target="imageTip">
                        Чтобы удалить фото - просто кликните по нему
                    </UncontrolledTooltip>
                    <Input type="file" name="imageFile" id="imageFile" accept=".jpg,.png,.jpeg" value={this.state.fileName} onChange={this.handleInputChange}/>
                    {imageBlock}
                </FormGroup>
                <div>
                    <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.editProfile()}>Сохранить</Button>{' '}
                    <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                </div>
            </Form>
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
            : this.renderProfile();

        return(
            <>
                {errorBaner}
                {content}
            </>
        )
    }

    async loadUser(userId) {
        const token = AuthHelper.getToken();
        fetch(`api/Users/${userId}/update`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            },
        })
        .then((response) => {
            if (!response.ok) {
                this.setState({
                    error: true
                });
            }
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data.message,
                });
            }
            else {
                this.setState({ 
                    id: data.id,
                    userName: data.userName,
                    contactEmail: data.contactEmail || 'Не указан',
                    contactPhone: data.contactPhone || 'Не указан',
                    hasImage: data.hasPhoto,
                    imagePath: data.photo,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    editProfile(){
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let formdata = new FormData();
        formdata.append('id', this.state.id);
        formdata.append('userName', this.state.userName);
        formdata.append('contactEmail', this.state.contactEmail);
        formdata.append('contactPhone', this.state.contactPhone);
        formdata.append('hasPhoto', this.state.hasImage);
        if (this.state.imageFile)
        {
            formdata.append('photo', this.state.imageFile);
        }
        const token = AuthHelper.getToken();
        fetch('api/Users', {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            body: formdata
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.id}`);
            }
            else {
                this.setState({error: true})
                return response.json();
            }
        }).then((data) => {
            if(this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}