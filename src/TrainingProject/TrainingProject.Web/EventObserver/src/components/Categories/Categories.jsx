import React, { Component } from 'react';
import { Table, Button } from 'reactstrap';
import { Link } from 'react-router-dom';

export default class Categories extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true, 
            error: false,
            categories: []
        };
    }

    componentDidMount() {
        this.loadCategories();
    }

    renderCategoriesList() {
        return(
            <>
            <Button color="primary" style={{marginBottom: '10px'}} tag={Link} to={"/newCategory"}>
                Добавить категорию
            </Button>
            <Table striped>
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Название</th>
                    </tr>
                </thead>
                <tbody>
                    {this.state.categories.map((e, index) => <tr key={e.id.toString()}><th scope="row">{index+1}</th><td><Link to={`category?id=${e.id}`}>{e.name}</Link></td></tr>)}
                </tbody>
            </Table>          
            </> 
        )
    }

    render() {
        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderCategoriesList(this.state.categories);

        return (
            <>
                <h2>Категории</h2>
                {content}
            </>
        );
    }

    async loadCategories() {
        fetch('api/Categories', {
            method: 'GET',
        })
        .then((response) => {
            this.setState({
                error: !response.ok
            });
            return response.json();
        }).then((data) => {
            if (this.state.error){
                console.log(data);
            }
            else {
                this.setState({ 
                    categories: data
                });
            }
        }).catch((ex) => {
            console.log(ex.toString());
        }).finally(() => {
            this.setState({ 
                loading: false 
            });
        })
    }
}