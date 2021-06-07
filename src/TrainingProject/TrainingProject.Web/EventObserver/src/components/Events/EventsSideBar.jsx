import React, { Component } from 'react';
import { Button, Form, FormGroup, Label, Input, Col } from 'reactstrap';
import { Link } from 'react-router-dom';
import DateTimeHelper from "../../Utils/dateTimeHelper";

export default class EventsSideBar extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            categories: [], 
            name: '', 
            category: 0, 
            tag: '', 
            from: '',
            to: ''
        };
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    handleInputChange(event) {
        const target = event.target;
        const name = target.name;
        const value = target.value;
        
        this.setState({
          [name]: value
        });
    }

    getQueryTrailer() {
        let isFirst = true
        let queryTrailer = '';
        if (this.state.name) {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `search=${this.state.name}`;
            isFirst = false;
        }
        if (this.state.category) {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `categoryId=${this.state.category}`;
            isFirst = false;
        }
        if (this.state.tag) {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `tag=${this.state.tag}`;
            isFirst = false;
        }
        if (this.state.from) {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `from=${this.state.from}`;
            isFirst = false;
        }
        if (this.state.to) {
            queryTrailer += isFirst ? '?' : '&';
            queryTrailer += `to=${this.state.to}`;
            isFirst = false;
        }

        return queryTrailer;
    }

    componentDidMount() {
        this.loadCategories();
    }

    render() {
    const categoriesSelect = this.state.categories.map(c => <option key={c.id.toString()} value={c.id}>{c.name}</option>)
    return (
        <Form>
            <FormGroup>
                <Label for="name">Название</Label>
                <Input type="text" name="name" id="name" value={this.state.name} onChange={this.handleInputChange}/>
            </FormGroup>
            <FormGroup> 
                <Label for="category">Категория</Label>
                <Input type="select" name="category" id="category" value={this.state.category} onChange={this.handleInputChange}>
                    {categoriesSelect}
                </Input>
            </FormGroup>
            <FormGroup>
                <Label for="tag">Тег</Label>
                <Input type="text" name="tag" id="tag" value={this.state.tag} onChange={this.handleInputChange}/>
            </FormGroup>
            <FormGroup>
                <Label for="dates">Даты</Label>
                <FormGroup name="dates" id="dates">
                    <FormGroup row>
                        <Label for="from" sm={2}>От</Label>
                        <Col sm={7} className="px-0">
                            <Input type="date" name="from" id="from" min={DateTimeHelper.getCurrentDate()}
                                   value={this.state.from} onChange={this.handleInputChange}/>
                        </Col>
                        <Col sm={3}>
                            <button type="button" className="btn btn-outline-secondary" disabled={!this.state.from}
                                    onClick={() => this.setState({from: ''})}>➖</button>
                        </Col>
                    </FormGroup>
                    <FormGroup row>
                        <Label for="to" sm={2}>До</Label>
                        <Col sm={7} className="px-0">
                            <Input type="date" name="to" id="to" min={DateTimeHelper.getCurrentDate()}
                                   value={this.state.to} onChange={this.handleInputChange}/>
                        </Col>
                        <Col sm={3}>
                            <button type="button" className="btn btn-outline-secondary" disabled={!this.state.to}
                                    onClick={() => this.setState({to: ''})}>➖</button>
                        </Col>
                    </FormGroup>
                </FormGroup>
            </FormGroup>
            <Button className="w-100" color="primary" tag={Link} to={`/events${this.getQueryTrailer()}`}>Поиск</Button>
        </Form>
    )}

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
}
