import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import { Link } from 'react-router-dom';
import queryString from 'query-string';
import PropTypes from 'prop-types';

export default class EventsSideBar extends Component {
    constructor(props) {
        super(props);
        this.state = { categories: [] };
        this.populateCategories = this.populateCategories.bind(this);
    }

    componentDidMount() {
        this.populateCategories();
    }

    render() {
    let renderCategories = this.state.categories.map(c => <option value={c.id}>{c.name}</option>)

        return (
            <Form>
                <FormGroup>
                    <Label for="name">Название</Label>
                    <Input type="text" name="name" id="name" />
                </FormGroup>
                <FormGroup>
                    <Label for="category">Категория</Label>
                    <Input type="select" name="category" id="category">
                        {renderCategories}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="tag">Тег</Label>
                    <Input type="text" name="tag" id="tag" />
                </FormGroup>
                <FormGroup check>
                    <Label check>
                    <Input type="checkbox" name="free" id="free"/>{' '}
                    Только бесплатные
                    </Label>
                </FormGroup>
                <FormGroup check>
                    <Label check>
                    <Input type="checkbox" name="vacancies" id="vacancies"/>{' '}
                    Есть свободные места
                    </Label>
                </FormGroup>
                <FormGroup tag="fieldset">
                    <FormGroup check>
                    <Label check>
                        <Input type="radio" name="actual"/>{' '}
                        Предстоящие
                    </Label>
                    </FormGroup>
                    <FormGroup check>
                    <Label check>
                        <Input type="radio" name="actual" />{' '}
                        Прошедшие
                    </Label>
                    </FormGroup>
                </FormGroup>
                <Button>Поиск</Button>
            </Form>           
        )
    }

    async populateCategories() {   
        const response = await fetch('api/Categories/Index');
        debugger;
        const data = await response.json();
        data.unshift({id: 0, name: 'Не выбрано'})
        this.setState({ categories: data });
    }
}
