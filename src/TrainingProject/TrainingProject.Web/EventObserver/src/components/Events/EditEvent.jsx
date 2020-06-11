import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert, UncontrolledTooltip } from 'reactstrap';
import queryString from 'query-string';
import AuthHelper from '../../Utils/AuthHelper'

export default class EditEvent extends Component{
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false,
            errorMessage: '',

            id: null,
            name: '', 
            category: '', 
            description: '', 
            place: '', 
            date: '', 
            time: '', 
            fee: 0, 
            participantsLimit: 0,
            tags: '', 
            imagePath: '', 
            imageFile: null,
            fileName: '',
            hasImage: false,
            formErrors: { 
                name: '', 
                category: '', 
                description: '', 
                place: '', 
                date: '', 
                time: '', 
                fee: '',
                participantsLimit: '',
                imageFile: '',
            },
            formValid: true,
            nameValid: true, 
            categoryValid: true, 
            descriptionValid: true, 
            placeValid: true, 
            dateValid: true, 
            timeValid: true, 
            feeValid: true,
            participantsLimitValid: true,
            tagsValid: true,
            imageFileValid: true,

            categories: [],
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.editEvent = this.editEvent.bind(this);
        this.cancel = this.cancel.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadEvent(parsed['id']);
        }
        this.loadCategories();
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        const value = target.value;

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

        let nameValid = this.state.nameValid;
        let categoryValid = this.state.categoryValid;
        let descriptionValid = this.state.descriptionValid;
        let placeValid = this.state.placeValid;
        let dateValid = this.state.dateValid;
        let timeValid = this.state.timeValid;
        let feeValid = this.state.feeValid;
        let participantsLimitValid = this.state.participantsLimitValid;
        let tagsValid = this.state.tagsValid;
        let imageFileValid = this.state.imageFileValid;

        switch(fieldName){
            case 'name':
                nameValid = !!value;
                fieldValidationErrors.name = nameValid ? '' : 'У мероприятия должно быть название';
                break;
            case 'category':
                categoryValid = !!value && value !== '0';
                fieldValidationErrors.category = categoryValid ? '' : 'Категория не выбрана';
                break;
            case 'description':
                descriptionValid = !!value;
                fieldValidationErrors.description = descriptionValid ? '' : 'У мероприятия должно быть описание';
                break;
            case 'place':
                placeValid = !!value;
                fieldValidationErrors.place = placeValid ? '' : 'Место проведение не указано';
                break;
            case 'date':
                dateValid = !!value;
                fieldValidationErrors.date = dateValid ? '' : 'Дата проведения не указана';
                break;
            case 'time':
                timeValid = !!value;
                fieldValidationErrors.time = timeValid ? '' : 'Время проведения не указана';
                break;
            case 'fee':
                feeValid = value.match(/^((0|([1-9][0-9]*))(\.[0-9]{0,2})?)$/i);
                fieldValidationErrors.fee = feeValid ? '' : 'Стоимость не указана или указана неверно';
                break;
            case 'participantsLimit':
                participantsLimitValid = value.match(/^((0|([1-9][0-9]{0,9})))$/i)
                fieldValidationErrors.participantsLimit = participantsLimitValid ? '' : 'Количество участников указано неверно';
                break;
            case 'tags':
                tagsValid = value.match(/^[\d\s\w,а-я]*$/i)
                fieldValidationErrors.tags = tagsValid ? '' : 'Допустимы только буквы, числа, пробелы, символы нижнего подчёркивания и запятые для разделения тегов';
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
            nameValid: nameValid,
            categoryValid: categoryValid,
            descriptionValid: descriptionValid,
            placeValid: placeValid,
            dateValid: dateValid,
            timeValid: timeValid,
            feeValid: feeValid,
            participantsLimitValid: participantsLimitValid,
            tagsValid: tagsValid,
            imageFileValid: imageFileValid,
          }, this.validateForm);
    }

    validateForm() {
        this.setState({
            formValid: 
                this.state.nameValid &&
                this.state.categoryValid &&
                this.state.placeValid &&
                this.state.dateValid &&
                this.state.timeValid &&
                this.state.feeValid &&
                this.state.participantsLimitValid &&
                this.state.tagsValid &&
                this.state.imageFileValid
        });
    }

    removeImage()
    {
        this.setState({
            imageFile: null,
            hasImage: false,
            fileName: '',
            imagePath: '',
            imageFileValid: true,
        }, this.validateForm)
    }

    cancel()
    {
        this.props.history.push(`/event?id=${this.state.id}`);
    }

    renderEvent()
    {
        const imageStyle = {
            maxWidth: '420px',
            maxHeight: '420px',
            marginTop: '8px',
        }

        const tipStyle = {
            color: '#70757A',
            textDecoration: 'underline'
        }

        const categoriesSelect = this.state.categories.map(c => <option key={c.id.toString()} value={c.id}>{c.name}</option>)
        let imageBlock;
        if (this.state.imageFile)
        {
            imageBlock = <img style={imageStyle} src={URL.createObjectURL(this.state.imageFile)} alt="event image" onClick={(e) => this.removeImage()}/>
        }
        else if (this.state.imagePath)
        {
            imageBlock = <img style={imageStyle} src={this.state.imagePath} alt="event image" onClick={(e) => this.removeImage()}/>
        }
        else imageBlock = null

        return(
            <>
            <h2>Редактирование информации о мероприятии</h2>
            <Form>
                <FormGroup>
                    <Label for="name">Название мероприятия</Label>
                    <Input invalid={!this.state.nameValid} required type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.name}</FormFeedback>
                </FormGroup>
                <FormGroup> 
                    <Label for="category">Категория</Label>
                    <Input invalid={!this.state.categoryValid} type="select" name="category" id="category" value={this.state.category} onChange={this.handleInputChange}>
                        {categoriesSelect}
                    </Input>
                    <FormFeedback>{this.state.formErrors.category}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="description">Описание</Label>
                    <Input invalid={!this.state.descriptionValid} required type="textarea" name="description" id="description" value={this.state.description} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.description}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="place">Место</Label>
                    <Input invalid={!this.state.placeValid} required type="text" name="place" id="place" value={this.state.place} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.place}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="date">Дата</Label>
                    <Input invalid={!this.state.dateValid} required type="date" name="date" id="date" value={this.state.date} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.date}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="time">Время</Label>
                    <Input invalid={!this.state.timeValid} required type="time" name="time" id="time" value={this.state.time} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.time}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="fee">Взнос</Label>{'  '}<span style={tipStyle} id="feeTip">?</span>
                    <UncontrolledTooltip placement="right" target="feeTip">
                        0 - бесплатно
                    </UncontrolledTooltip>
                    <Input invalid={!this.state.feeValid} required type="text" name="fee" id="fee" value={this.state.fee} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.fee}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="participantsLimit">Количество участников</Label>{'  '}<span style={tipStyle} id="limitTip">?</span>
                    <UncontrolledTooltip placement="right" target="limitTip">
                        0 - без ограничений
                    </UncontrolledTooltip>
                    <Input invalid={!this.state.participantsLimitValid} required type="number" name="participantsLimit" id="participantsLimit" value={this.state.participantsLimit} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.participantsLimit}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="tags">Теги</Label>{'  '}<span style={tipStyle} id="tagsTip">?</span>
                    <UncontrolledTooltip placement="right" target="tagsTip">
                        Теги могут содержать буквы, цифры, пробелы, знаки нижнего подчёркивания и разделяются запятыми
                    </UncontrolledTooltip>
                    <Input invalid={!this.state.tagsValid} type="text" name="tags" id="tags" value={this.state.tags} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.tags}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="imageFile">Картинка</Label>{'  '}<span style={tipStyle} id="imageTip">?</span>
                    <UncontrolledTooltip placement="right" target="imageTip">
                        Чтобы удалить картинку - просто кликните по ней
                    </UncontrolledTooltip>
                    <Input invalid={!this.state.imageFileValid} type="file" name="imageFile" id="imageFile" accept=".jpg,.png,.jpeg" value={this.state.fileName} onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.imageFile}</FormFeedback>
                    {imageBlock}
                </FormGroup>
                <div>
                    <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.editEvent()}>Сохранить</Button>{' '}
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
            : this.renderEvent();

        return(
            <>
                {errorBaner}
                {content}
            </>
        )
    }

    loadCategories() {
        fetch('api/Categories')
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
                    data.unshift({
                        id: 0, 
                        name: 'Не выбрано'
                    })
                    this.setState({ 
                        categories: data 
                    });
                }
            }).catch((ex) => {
                this.setState({
                    errorMessage: ex.toString()
                });
            });
    }

    async loadEvent(eventId) {
        const token = await AuthHelper.getToken();
        fetch('api/Events/' + eventId + '/update', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
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
                    category: data.categoryId,
                    description: data.description,
                    date: data.date,
                    time: data.time,
                    place: data.place,
                    fee: data.fee,
                    participantsLimit: data.participantsLimit,
                    tags: data.tags,
                    imagePath: data.image,
                    hasImage: !!data.image,
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    async editEvent()
    {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let formdata = new FormData();
        formdata.append('id', this.state.id);
        formdata.append('name', this.state.name);
        formdata.append('categoryId', this.state.category);
        formdata.append('description', this.state.description);
        const dateTime = new Date(this.state.date + " " + this.state.time);
        const start = `${dateTime.getDate()}/${dateTime.getMonth()+1}/${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`
        formdata.append('start', start);
        formdata.append('place', this.state.place);
        formdata.append('fee', parseFloat(this.state.fee));
        formdata.append('participantsLimit', this.state.participantsLimit);
        formdata.append('hasImage', this.state.hasImage);
        if (this.state.tags) {
            formdata.append('tags', this.state.tags);
        }
        if (this.state.imageFile) {
            formdata.append('image', this.state.imageFile);
        }
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch('api/Events', {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            body: formdata
        }).then((response) => {
            if (response.ok){
                this.props.history.push(`/event?id=${this.state.id}`);
            }
            else {
                this.setState({error: true});
                return response.json();
            }
        }).then((data) => {
            if (this.state.error)
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