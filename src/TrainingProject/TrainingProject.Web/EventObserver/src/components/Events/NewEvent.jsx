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
            dateTime: null,
            weekDays: {},
            fee: 0, 
            participantsLimit: 0,
            tags: '',
            imageFile: null,
            fileName: '',
            isRecurrent: false,
            formErrors: { 
                name: '', 
                category: '', 
                description: '', 
                place: '',
                dateTime: '',
                weekDays: '',
                fee: '',
                participantsLimit: '',
                imageFile: ''
            },
            formValid: false,
            nameValid: false,
            categoryValid: false,
            descriptionValid: false,
            placeValid: false,
            dateTimeValid: false,
            weekDaysValid: false,
            feeValid: true,
            participantsLimitValid: true,
            tagsValid: true,
            imageFileValid: true,

            error: false,
            errorMessage: '',
            categories: [],
        };
        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleWeekdayTimeChange = this.handleWeekdayTimeChange.bind(this);
        this.onWeekdayTimeClear = this.onWeekdayTimeClear.bind(this);
        this.validateField = this.validateField.bind(this);
        this.createEvent = this.createEvent.bind(this);
        this.cancel = this.cancel.bind(this);
    }

    componentDidMount() {
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
            }, 
                () => { this.validateField(name, target.files[0]) 
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

    validateField(fieldName, value= null) {
        let fieldValidationErrors = this.state.formErrors;

        let nameValid = this.state.nameValid;
        let categoryValid = this.state.categoryValid;
        let descriptionValid = this.state.descriptionValid;
        let placeValid = this.state.placeValid;
        let dateTimeValid = this.state.dateTime;
        let weekDaysValid = this.state.weekDaysValid;
        let feeValid = this.state.feeValid;
        let participantsLimitValid = this.state.participantsLimitValid;
        let tagsValid = this.state.tagsValid;
        let imageFileValid = this.state.imageFileValid;

        switch(fieldName) {
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
            case 'dateTime':
                dateTimeValid = !!value;
                fieldValidationErrors.dateTime = dateTimeValid ? '' : 'Время и дата проведения не указана';
                break;
            case 'weekDays':
                weekDaysValid = Object.values(this.state.weekDays).some(x => !!x);
                fieldValidationErrors.weekDays = weekDaysValid ? '' : 'Нужно выбрать хотя бы один день';
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
                tagsValid = value.match(/^[\d\s\w,а-я]*$/ui)
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
            dateTimeValid: dateTimeValid,
            weekDaysValid: weekDaysValid,
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
                (!this.state.isRecurrent && this.state.dateTimeValid
                    || this.state.isRecurrent && this.state.weekDaysValid) &&
                this.state.feeValid &&
                this.state.participantsLimitValid &&
                this.state.tagsValid &&
                this.state.imageFileValid
        });
    }

    removeImage() {
        this.setState({
            imageFile: null,
            fileName: '',
            imageFileValid: true,
        }, this.validateForm)
    }

    cancel() {
        this.props.history.push(`/events`);
    }

    handleWeekdayTimeChange(event) {
        const target = event.target;
        const weekday = parseInt(target.dataset.weekday);
        
        this.setState({weekDays: {...this.state.weekDays, [weekday]: target.value}}, 
            () => this.validateField('weekDays'));
    }

    onWeekdayTimeClear(event) {
        const target = event.target;
        const weekday = parseInt(target.dataset.weekday);

        this.setState({weekDays: {...this.state.weekDays, [weekday]: ''}},
            () => this.validateField('weekDays'));
    }

    render() {
        const imageStyle = {
            maxWidth: '420px',
            maxHeight: '420px',
            marginTop: '8px',
        }

        const tipStyle = {
            color: '#70757A',
            textDecoration: 'underline'
        }

        const errorBanner = this.state.errorMessage ? 
            <Alert color="danger">
                {this.state.errorMessage}
            </Alert> : null;

        const categoriesSelect = this.state.categories.map(c => <option key={c.id.toString()} value={c.id}>{c.name}</option>)
        const imageBlock = this.state.imageFile ? 
            <img style={imageStyle} src={URL.createObjectURL(this.state.imageFile)} alt="event image" 
                 onClick={(e) => this.removeImage()}/> : null;
        const datePicker = this.state.isRecurrent ?
            <FormGroup>
                <table className="table table-borderless">
                    <tbody> {
                        ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'].map((day, index) => {
                                let key = (index+1) % 7;
                                
                                return(
                                    <tr>
                                        <td style={{width: 'fit-content'}}>{day}</td>
                                        <td>
                                            <Input invalid={!this.state.weekDaysValid} type="time" data-weekday={key} 
                                                   value={this.state.weekDays[key]} onChange={this.handleWeekdayTimeChange}/>
                                        </td>
                                        <td style={{width: '72px'}}> {
                                                this.state.weekDays[key] ?
                                                    <button className="btn btn-outline-secondary" data-weekday={key} 
                                                            onClick={this.onWeekdayTimeClear}>➖</button> : null
                                            }
                                        </td>
                                    </tr>
                                )
                            })
                        }
                    </tbody>
                </table>
                <div className="text-danger">{this.state.formErrors.weekDays}</div>
            </FormGroup> :
            <FormGroup>
                <Label for="dateTime">Дата и время</Label>
                <Input invalid={!this.state.dateTimeValid} required type="datetime-local" name="dateTime" id="dateTime"
                       value={this.state.dateTime} onChange={this.handleInputChange}/>
                <FormFeedback>{this.state.formErrors.dateTime}</FormFeedback>
            </FormGroup>

        return(
            <div className="mx-auto" style={{maxWidth: '720px'}}>
                {errorBanner}
                <div className="list-group">
                    <div className="list-group-item bg-light">
                        <h3 className="m-0">Новое мероприятие</h3>
                    </div>
                    <div className="list-group-item">
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
                            <ul className="nav nav-tabs">
                                <li className="nav-item">
                                    <a type="button" className={`nav-link ${this.state.isRecurrent ? null : 'active'}`}
                                       onClick={() => this.setState({isRecurrent : false}, () => this.validateForm())}>
                                        Разовое мероприятие
                                    </a>
                                </li>
                                <li className="nav-item">
                                    <a type="button" className={`nav-link ${this.state.isRecurrent ? 'active' : null}`}
                                       onClick={() => this.setState({isRecurrent : true}, () => this.validateForm())}>
                                        Рекурентное мероприятие
                                    </a>
                                </li>
                            </ul>
                            {datePicker}
                            <hr/>
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
                                <Button disabled = {!this.state.formValid} color="primary" onClick={() => this.createEvent()}>Опубликовать</Button>{' '}
                                <Button color="secondary" onClick={() => this.cancel()}>Отменить</Button>
                            </div>
                        </Form>
                    </div>
                </div>
            </div>
        )
    }

    async loadCategories() {
        fetch('api/categories/names')
            .then((response) => {
                this.setState({
                    error: !response.ok
                });
                return response.json();
            }).then((data) => {
                if (this.state.error) {
                    console.log(data);
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
                console.log(ex.toString)
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

        let formData = new FormData();

        formData.append('name', this.state.name);
        formData.append('categoryId', this.state.category);
        formData.append('description', this.state.description);
        formData.append('place', this.state.place);
        formData.append('fee', parseFloat(this.state.fee));
        formData.append('participantsLimit', this.state.participantsLimit);
        formData.append('organizerId', AuthHelper.getId());
        formData.append('isRecurrent', this.state.isRecurrent);
        
        if (this.state.tags) {
            formData.append('tags', this.state.tags);
        }
        
        if (this.state.imageFile) {
            formData.append('image', this.state.imageFile);
        }

        if (this.state.isRecurrent) {
            const weekDays = Object.keys(this.state.weekDays).map(key => ({
                dayOfWeek: key,
                start: this.state.weekDays[key]
            }));

            formData.append('eventDaysOfWeek', JSON.stringify(weekDays));
        }
        else {
            const dateTime = new Date(this.state.dateTime);
            const start = `${dateTime.getDate()}/${dateTime.getMonth()+1}/${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`;

            formData.append('start', start);
        }

        AuthHelper.fetchWithCredentials('api/events', {
            method: 'POST',
            body: formData
        }).then((response) => {
            if (response.ok){
                this.props.history.push("/events");
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
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