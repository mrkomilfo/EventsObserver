import React, { Component } from 'react';
import queryString from 'query-string';
import EventMedia from './EventMedia';
import EventsPaginator from './EventsPaginator';
import EventsSideBar from './EventsSideBar';
import { Alert } from 'reactstrap';

export default class Events extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            query: window.location.search, events: [], currentPage: 0, pageSize: 12, totalRecords: 0, loading: true, 
            error: false, errorMessage: '' 
        };
        debugger;
    }

    componentDidMount() {
        this.populateEventsData();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.query != window.location.search) {
            this.setState({ query: window.location.search });
            this.populateEventsData();
        }
    }

    renderEventsList(events){
        return(
            <div>
                <p>{`Найдено событий: ${this.state.totalRecords}`}</p>
                <ul>
                    {this.state.events.map(e => <li style={{listStyleType: 'none'}} key={e.id}><EventMedia event={e}/><hr/></li>)}
                </ul>
                <EventsPaginator currentPage={this.state.currentPage} totalPages={Math.ceil(this.state.totalRecords / this.state.pageSize)}/>
            </div>             
        )
    }

    render() {
        const errorBaner = this.state.error ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEventsList(this.state.events);

        const pageStyle = {
            display: 'flex'
        }
        const contentStyle = {
            width: '100%'
        }
        const filterStyle = {
            float: 'right',
            width: '240px',
            margin: '0px 0px 0px 32px'
        }
    
        return (
            <>
            {errorBaner}
            <div style={pageStyle}>
                <div style={contentStyle}>
                    <h2 id="tabelLabel">События в Минске</h2>
                    {contents}
                </div>
                <div style={filterStyle}>
                    <EventsSideBar />
                </div>
            </div>
            </>
        );
    }

    async populateEventsData() {
        let page; let search; let categoryId; let tag; let upComing; let onlyFree; let vacancies; let organizer; let participant;
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            page = parsed['page'] || 0;
            search = parsed['search'];
            categoryId = parsed['categoryId'];
            tag = parsed['tag'];
            upComing = parsed['upComing'];
            onlyFree = parsed['onlyFree'];
            vacancies = parsed['vacancies'];
            organizer = parsed['organizer'];
            participant = parsed['participant'];
        }

        let queryTrailer = '?page=' + page;
        if (search) {
            queryTrailer += `&search=${search}`
        }
        if (categoryId) {
            queryTrailer += `&categoryId=${categoryId}`
        }
        if (tag) {
            queryTrailer += `&tag=${tag}`
        }
        if (upComing) {
            queryTrailer += `&upComing=${upComing}`
        }
        if (onlyFree) {
            queryTrailer += `&onlyFree=${onlyFree}`
        }
        if (vacancies) {
            queryTrailer += `&vacancies=${vacancies}`
        }
        if (organizer) {
            queryTrailer += `&organizer=${organizer}`
        }
        if (participant) {
            queryTrailer += `&participant=${participant}`
        }

        fetch('api/Events' + queryTrailer)
            .then((response) => {
                if (!response.ok) {
                    this.setState({error: true});
                }
                return response.json();
            }).then((data) => {
                if (this.state.error){
                    this.setState({errorMessage: data.message});
                }
                else {
                    this.setState({ events: data.records, currentPage: data.currentPage, pageSize: data.pageSize, 
                        totalRecords: data.totalRecords, loading: false });
                }
            }).catch((ex) => {
                this.setState({errorMessage: ex.toString()});
            });
    }
}