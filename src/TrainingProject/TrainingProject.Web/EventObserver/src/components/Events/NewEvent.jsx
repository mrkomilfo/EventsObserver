import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormFeedback, Alert, UncontrolledTooltip } from 'reactstrap';
import AuthHelper from '../../Utils/authHelper'

export default class NewEvent extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            name: '', 
            category: '', 
            description: '', 
            place: '', 
            date: null, 
            time: null, 
            fee: 0, 
            participantsLimit: 0,
            tags: '',
            imageFile: null,
            fileName: '',
            formErrors: { 
                name: '', 
                category: '', 
                description: '', 
                place: '', 
                date: '', 
                time: '', 
                fee: 0,
                participantsLimit: 0
            },
            formValid: false,
            nameValid: false, 
            categoryValid: false, 
            descriptionValid: false, 
            placeValid: false, 
            dateValid: false, 
            timeValid: false, 
            feeValid: true,
            participantsLimitValid: true,
            tagsValid: true,

            error: false,
            errorMessage: '',
            categories: [],
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.validateField = this.validateField.bind(this);
        this.createEvent = this.createEvent.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        const value = target.value;

        if (name == 'imageFile')
        {
            this.setState({
                fileName: value,
                imageFile: target.files[0],
            }); 
        }

        else{
            this.setState({
                [name]: value
            }, 
                () => { this.validateField(name, value) 
            });
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

        switch(fieldName){
            case 'name':
                nameValid = !!value;
                fieldValidationErrors.name = nameValid ? '' : 'У мероприятия должно быть название';
                break;
            case 'category':
                categoryValid = !!value && value != '0';
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
                tagsValid = value.match(/^[\d\s\w\,]*$/u)
                fieldValidationErrors.tags = tagsValid ? '' : 'Допустимы только буквы, числа, пробелы, символы нижнего подчёркивания и запятые для разделения тегов';
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
                this.state.tagsValid
        });
    }

    removeImage()
    {
        this.setState({
            fileName: '', 
            imageFile: null
        })
    }

    componentDidMount() {
        this.loadCategories();
    }

    render(){
        const imageStyle = {
            maxWidth: '420px',
            maxHeight: '420px',
            marginTop: '8px',
        }

        const tipStyle = {
            color: '#70757A',
            textDecoration: 'underline'
        }

        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const categoriesSelect = this.state.categories.map(c => <option key={c.id.toString()} value={c.id}>{c.name}</option>)
        const imageBlock = this.state.imageFile ? <img style={imageStyle} src={URL.createObjectURL(this.state.imageFile)} alt="event image" onClick={(e) => this.removeImage()}/> : null;

        return(
            <>
            {errorBaner}
            <Form>
                <h2>Новое мероприятие</h2>
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
                    <Input invalid={!this.state.dateValid} required type="date" name="date" id="date" onChange={this.handleInputChange}/>
                    <FormFeedback>{this.state.formErrors.date}</FormFeedback>
                </FormGroup>
                <FormGroup>
                    <Label for="time">Время</Label>
                    <Input invalid={!this.state.timeValid} required type="time" name="time" id="time" onChange={this.handleInputChange}/>
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
                    <Input type="file" name="imageFile" id="imageFile" accept=".jpg,.png,.jpeg" value={this.state.fileName} onChange={this.handleInputChange}/>
                    {imageBlock}
                </FormGroup>
                <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.createEvent()}>Опубликовать</Button>
            </Form>
            </>
        )
    }

    loadCategories() {
        fetch('api/Categories')
            .then((response) => {
                if (!response.ok) {
                    this.setState({error: true});
                }
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

    createEvent()
    {
        if (!this.state.formValid)
        {
            this.setState({
                errorMessage: 'Форма не валидна'
            })
            return;
        }
        let formdata = new FormData();
        formdata.append('name', this.state.name);
        formdata.append('categoryId', this.state.category);
        formdata.append('description', this.state.description);

        const dateTime = new Date(this.state.date + " " + this.state.time);
        const start = `${dateTime.getDate()}/${dateTime.getMonth()+1}/${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`
        formdata.append('start', start);

        formdata.append('place', this.state.place);
        formdata.append('fee', parseFloat(this.state.fee));
        formdata.append('participantsLimit', this.state.participantsLimit);
        formdata.append('organizerId', AuthHelper.getId());

        if (this.state.tags)
        {
            const allTags = this.state.tags.split(',').map((tag) => tag.trim().toLowerCase());
            const uniqueTags = allTags.filter((item, pos) => { return allTags.indexOf(item) == pos; });  
            formdata.append('tags', JSON.stringify(uniqueTags));
        }

        if (this.state.imageFile)
        {
            formdata.append('image', this.state.imageFile);
        }
        const token = AuthHelper.getToken();
        fetch('api/Events', {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            body: formdata
        }).then((response) => {
            if (response.ok){
                this.props.history.push("/events");
            }
            else {
                return response.json();
            }
        }).then((data) => {
            this.setState({
                errorMessage: data.message
            });
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }
}