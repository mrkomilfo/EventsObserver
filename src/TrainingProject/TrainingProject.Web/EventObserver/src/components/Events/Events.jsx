﻿import React, { Component } from 'react';
import queryString from 'query-string';
import EventMedia from './EventMedia';
import EventsPaginator from './EventsPaginator';
import EventsSideBar from './EventsSideBar';

export default class Events extends Component {
    constructor(props) {
        super(props);
        this.state = { 
            loading: true,

            error: false, 

            query: window.location.search, 
            events: [], 
            currentPage: 0, 
            pageSize: 8, 
            totalRecords: 0,  
        };
    }

    componentDidMount() {
        this.loadEvents();
    }

    componentWillReceiveProps(nextProps) {
        if (this.state.query !== window.location.search) {
            this.setState({ query: window.location.search });
            this.loadEvents();
        }
    }

    renderEventsList(events) {
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
        const content = this.state.loading
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
            <div style={pageStyle}>
                <div style={contentStyle}>
                    <h2 id="tabelLabel">События в Минске</h2>
                    {content}
                </div>
                <div style={filterStyle}>
                    <EventsSideBar />
                </div>
            </div>
        );
    }

    async loadEvents() {
        let page; let search; let categoryId; let tag; let upComing; let onlyFree; let vacancies; let organizerId; let participantId;
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            page = parsed['page'] || 0;
            search = parsed['search'];
            categoryId = parsed['categoryId'];
            tag = parsed['tag'];
            upComing = parsed['upComing'];
            onlyFree = parsed['onlyFree'];
            vacancies = parsed['vacancies'];
            organizerId = parsed['organizerId'];
            participantId = parsed['participantId'];
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
        if (organizerId) {
            queryTrailer += `&organizerId=${organizerId}`
        }
        if (participantId) {
            queryTrailer += `&participantId=${participantId}`
        }

        fetch(`api/Events${queryTrailer}`)
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
                        events: data.records, 
                        currentPage: data.currentPage, 
                        pageSize: data.pageSize, 
                        totalRecords: data.totalRecords
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
}