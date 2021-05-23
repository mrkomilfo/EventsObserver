import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert, UncontrolledTooltip } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/authHelper.js';

export default class EditProfile extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false, 
            errorMessage: '',
            noContent: false,

            id: '', 
            userName: '',
            contactPhone: '', 
            hasImage: false, 
            imagePath: '', 
            imageFile: null, 
            fileName: '',
            formErrors: { 
                userName: '',
                contactPhone: '',
                imageFile: ''
            },
            formValid: true,
            userNameValid: true,
            contactPhoneValid: true,
            imageFileValid: true,
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

        if (name === 'imageFile')
        {
            this.setState({
                fileName: value,
                imageFile: target.files[0],
                hasImage: true
            }, 
                () => { this.validateField(name, target.files[0]) }
            )
        }
        else {
            this.setState({
                [name]: value
            }, 
                () => { this.validateField(name, value) }
            );
        }
    }

    validateField(fieldName, value) {
        let fieldValidationErrors = this.state.formErrors;

        let userNameValid = this.state.userNameValid;
        let contactPhoneValid = this.state.contactPhoneValid;
        let imageFileValid = this.state.imageFileValid;

        switch(fieldName){
            case 'userName':
                userNameValid = value.length >= 4;
                fieldValidationErrors.userName = userNameValid ? '' : 'Минимальная длина - 4';
                break;
            case 'contactPhone':
                contactPhoneValid = value.match(/^\+?[0-9]{6,12}$/i) || value.length === 0;
                fieldValidationErrors.contactPhone = contactPhoneValid ? '' : 'Неверный формат';
                break;
            case 'imageFile':
                imageFileValid = value.size <= 8388608 //8 Mb
                fieldValidationErrors.imageFile = imageFileValid ? '' : 'Размер изображения не должен превышать 8 Mb';
                break;
            default:
                break;
        }
        this.setState({
            formErrors: fieldValidationErrors,
            userNameValid,
            contactPhoneValid,
            imageFileValid,
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.userNameValid &&
                this.state.contactPhoneValid
        });
    }

    removeImage() {
        this.setState({
            imageFile: null,
            hasImage: false,
            fileName: '',
            imagePath: '',
            imageFileValid: true,
        }, this.validateForm)
    }

    cancel() {
        this.props.history.push(`/user?id=${this.state.id}`);
    }

    renderProfile() {
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
        if (this.state.imageFile) {
            imageBlock = <img style={imageStyle} src={URL.createObjectURL(this.state.imageFile)} alt="profile photo" onClick={(e) => this.removeImage()}/>
        }
        else if (this.state.imagePath) {
            imageBlock = <img style={imageStyle} src={this.state.imagePath} alt="profile photo" onClick={(e) => this.removeImage()}/>
        }
        else {
            imageBlock = <img style={imageStyle} src='img\users\default.jpg' alt="profile photo" onClick={(e) => this.removeImage()}/>
        }

        return(
            <Form>
                <FormGroup>
                    <Label for="userName">Имя пользователя</Label>
                    <Input invalid={!this.state.userNameValid} required type="text" name="userName" id="userName" value={this.state.userName} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.userName}</FormFeedback>
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
                    <Input invalid={!this.state.imageFileValid} type="file" name="imageFile" id="imageFile" accept=".jpg,.png,.jpeg" value={this.state.fileName} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.imageFile}</FormFeedback>
                    {imageBlock}
                </FormGroup>
                <div>
                    <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.editProfile()}>Сохранить</Button>{' '}
                    <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                </div>
            </Form>
        )
    }

    render() {
        const errorBaner = !this.state.errorMessage ? null :
            <Alert color="danger">
                {this.state.errorMessage}
            </Alert>;

        return(
            this.state.loading ? <p><em>Loading...</em></p> :
            (this.state.noContent ? 
                <Alert color="info">
                    {"Пользователь удалён или ещё не зарегистрирован"}
                </Alert> : 
                <div className="mx-auto" style={{maxWidth: '720px'}}>
                    {errorBaner}
                    <div className="list-group">
                        <div className="list-group-item bg-light">
                            <h3 id="tabelLabel">Редактирование профиля</h3>
                        </div>
                        <div className="list-group-item">
                            {this.renderProfile()}
                        </div>
                    </div>
                </div>
            )
        )
    }

    async loadUser(userId) {
        AuthHelper.fetchWithCredentials(`api/users/${userId}/update`)
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
                        userName: data.userName,
                        contactPhone: data.contactPhone || "",
                        hasImage: data.hasPhoto,
                        imagePath: data.photo
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

    async editProfile() {
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
        formdata.append('contactPhone', this.state.contactPhone);
        formdata.append('hasPhoto', this.state.hasImage);
        if (this.state.imageFile)
        {
            formdata.append('photo', this.state.imageFile);
        }
        AuthHelper.fetchWithCredentials('api/users', {
            method: 'PUT',
            body: formdata
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/user?id=${this.state.id}`);
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                this.setState({
                    error: true
                })
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