import React, { Component } from 'react';
import queryString from 'query-string';
import EventRow from './EventRow';
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

    render() {
        return (
            this.state.loading ? <p><em>Loading...</em></p> :
            <div style={{display: 'flex'}}>
                <div className="list-group mr-3">
                    <div className="list-group-item bg-light">
                        <h3 id="tabelLabel">События в Минске</h3>
                    </div>
                    <div className="list-group-item">
                        <table className="table m-0">
                            {this.state.events.map(e => <EventRow event={e}/>)}
                        </table>
                    </div>
                    <div className="list-group-item pb-0">
                        <EventsPaginator 
                            currentPage={this.state.currentPage} 
                            totalPages={Math.ceil(this.state.totalRecords / this.state.pageSize)}
                        />
                    </div>
                </div>
                <div className="list-group">
                    <div className="list-group-item">
                        <EventsSideBar />
                    </div>
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