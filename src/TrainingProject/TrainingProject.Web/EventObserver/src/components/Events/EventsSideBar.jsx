import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input } from 'reactstrap';
import { Link } from 'react-router-dom';

export default class EventsSideBar extends Component {
    constructor(props) {
        super(props);
        this.state = { categories: [], name: '', category: 0, tag: '', free: false, vacancies: false, upComing: null };
        this.populateCategories = this.populateCategories.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.getQuerryTrailer = this.getQuerryTrailer.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        let value;
        switch(name)
        {
            case 'free': 
            case 'vacancies':
                value = target.checked;
                break;
            case 'upComing':
                switch(target.value)
                {
                    case 'true':
                        value = true
                        break;
                    case 'false':
                        value = false
                        break;
                    default:
                        value = null;
                }
                break;
            default:
                value = target.value
        }
        
        this.setState({
          [name]: value
        });
    }

    getQuerryTrailer(){
        let isFirst = true
        let queryTrailer = '';
        if (this.state.name)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `search=${this.state.name}`;
            isFirst = false;
        }
        if (this.state.category != 0)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `categoryId=${this.state.category}`;
            isFirst = false;
        }
        if (this.state.tag)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `tag=${this.state.tag}`;
            isFirst = false;
        }
        if (this.state.free)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `onlyFree=${this.state.free}`;
            isFirst = false;
        }
        if (this.state.vacancies)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `vacancies=${this.state.vacancies}`;
            isFirst = false;
        }
        if (this.state.upComing != null)
        {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `upComing=${this.state.upComing}`;
            isFirst = false;
        }
        return queryTrailer;
    }

    componentDidMount() {
        this.populateCategories();
    }

    render() {
        const actualityStyle = {
            marginTop: '16px'
        }

    let renderCategories = this.state.categories.map(c => <option value={c.id}>{c.name}</option>)
        return (
            <Form>
                <FormGroup>
                    <Label for="name">Название</Label>
                    <Input type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup> 
                    <Label for="category">Категория</Label>
                    <Input type="select" name="category" id="category" value={this.state.category} onChange={this.handleInputChange}>
                        {renderCategories}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="tag">Тег</Label>
                    <Input type="text" name="tag" id="tag" value={this.state.tag} onChange={this.handleInputChange}/>
                </FormGroup>
                <FormGroup check>
                    <Label check>
                    <Input type="checkbox" name="free" id="free" checked={this.state.free} onChange={this.handleInputChange}/>{' '}
                    Только бесплатные
                    </Label>
                </FormGroup>
                <FormGroup check>
                    <Label check>
                    <Input type="checkbox" name="vacancies" id="vacancies" checked={this.state.vacancies} onChange={this.handleInputChange}/>{' '}
                    Есть свободные места
                    </Label>
                </FormGroup>
                <FormGroup tag="fieldset" style={actualityStyle}>
                    <Label>
                    Актуальность
                    <FormGroup check>
                        <Label check>
                            <Input type="radio" name="upComing" value={'true'} checked={this.state.upComing === true} onChange={this.handleInputChange}/>{' '}                               
                            Предстоящие
                        </Label>
                    </FormGroup>
                    <FormGroup check>
                        <Label check>
                            <Input type="radio" name="upComing" value={'false'} checked={this.state.upComing === false} onChange={this.handleInputChange}/>{' '}
                            Прошедшие
                        </Label>
                    </FormGroup>
                    <FormGroup check>
                        <Label check>
                            <Input type="radio" name="upComing" value={'null'} checked={this.state.upComing === null} onChange={this.handleInputChange}/>{' '}
                            Все
                        </Label>
                    </FormGroup>
                    </Label> 
                </FormGroup>
                <Button tag={Link} to={`/events${this.getQuerryTrailer()}`}>Поиск</Button>
            </Form>
        )
    }

    async populateCategories() {
        const response = await fetch('api/Categories');
        const data = await response.json();
        data.unshift({id: 0, name: 'Не выбрано'})
        this.setState({ categories: data });
    }
}
