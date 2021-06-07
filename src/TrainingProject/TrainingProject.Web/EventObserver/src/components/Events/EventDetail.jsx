import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Link } from 'react-router-dom';
import Parser from 'html-react-parser';

import AuthHelper from '../../Utils/authHelper.js';
import ErrorPage from '../Common/ErrorPage';
import CommentsBlock from '../Comments/CommentsBlock';
import DateTimeHelper from '../../Utils/dateTimeHelper';

export default class EventDetail extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,

            error: false,
            statusCode: 200,

            id: 0,
            name: '',
            categoryId: null,
            category: '',
            description: '',
            start: '',
            startParsable: 0,
            place: '',
            fee: 0,
            participants: 0,
            participantsLimit: 0,
            organizerId: '',
            organizer: '',
            tags: [],
            publicationTime: '',
            image: '',
            amISubscribed: null,
            isActive: null,
            isApproved: null,
            participantsList: [],
            
            isRecurrent: null,
            weekDays: [],
            activeWeekDayId: null,

            deleteModal: false,

            userRole: AuthHelper.getRole(),
            userId: AuthHelper.getId(),
        }
        this.loadParticipantsIfNecessary = this.loadParticipantsIfNecessary.bind(this);
        this.deleteEvent = this.deleteEvent.bind(this);
        this.subscribe = this.subscribe.bind(this);
        this.unsubscribe = this.unsubscribe.bind(this);
        this.subscribeRecurrent = this.subscribeRecurrent.bind(this);
        this.unsubscribeRecurrent = this.unsubscribeRecurrent.bind(this);
        this.toggleApprove = this.toggleApprove.bind(this);
        this.toggleDeleteModal = this.toggleDeleteModal.bind(this);
    }

    async componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed && parsed['id']) {
            await this.loadData(parsed['id']);
        }
        else {
            this.setState({
                statusCode: 404
            });
        }
    }

    toggleDeleteModal() {
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    changeWeekDay(e) {
        this.setState({activeWeekDayId: parseInt(e.target.dataset.id)}, this.loadParticipantsIfNecessary);
    }

    getFreePlaces(participants) {
        return this.state.participantsLimit
            ? `${this.state.participantsLimit - participants}/${this.state.participantsLimit}`
            : 'Не ограничено';
    }

    isOrganizerOrAdmin() {
        return this.state.userRole === "Admin" || this.state.organizerId === this.state.userId;
    }

    async loadParticipantsIfNecessary() {
        if (this.isOrganizerOrAdmin()) {
            await this.loadParticipants();
        }
    }

    renderButtonPanel() {
        const approveButton = this.state.userRole !== 'Admin' ? null :
            this.state.isApproved ?
                <Button color="success" onClick={this.toggleApprove}>
                    Подтверждено
                </Button> :
                <Button color="primary" onClick={this.toggleApprove}>
                    Подтвердить
                </Button>;
        const editButton = !this.state.isApproved && this.state.userId === this.state.organizerId || this.state.userRole === 'Admin' ?
            <Button className="ml-2" outline color="primary" tag={Link} to={`/editEvent?id=${this.state.id}`}>
                <i className="bi bi-pencil"/>
            </Button> : null

        const deleteButton = this.state.userId === this.state.organizerId || this.state.userRole === 'Admin' ? 
            <Button className="ml-2" outline color="danger" onClick={this.toggleDeleteModal}>
                <i className="bi bi-trash"/>
            </Button> : null;

        const deleteModal = deleteButton ?
            <Modal isOpen={this.state.deleteModal} toggle={this.toggleDeleteModal}>
                <ModalHeader toggle={this.toggleDeleteModal}>Подтвердите действие</ModalHeader>
                <ModalBody>
                    Вы действительно хотите удалить данное событие?
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.deleteEvent}>Да</Button>{' '}
                    <Button color="secondary" onClick={this.toggleDeleteModal}>Отмена</Button>
                </ModalFooter>
            </Modal> : null

        return(
            <div style={{minWidth: "fit-content"}}>
                {approveButton}
                {editButton}
                {deleteButton}
                {deleteModal}
            </div>
        )
    }

    renderSubscribeButton()
    {
        if (this.state.userRole === 'Guest' || this.state.organizerId === this.state.userId) {
            return null;
        }

        if (!this.state.amISubscribed) {
            return <button className="btn btn-sm btn-outline-primary" onClick={this.subscribe}
                           disabled={!this.state.isActive || (this.state.participantsLimit !== 0
                                    && this.state.participants >= this.state.participantsLimit)}>Записаться</button>
        }
        else {
            return <button className="btn btn-sm btn-outline-secondary" onClick={this.unsubscribe}
                           disabled={!this.state.isActive}>Отписаться</button>
        }
    }

    renderSubscribeButtonForRecurrent(amISubscribed, haveFreePlaces, eventWeekDayId) {
        if (this.state.userRole === 'Guest' || this.state.organizerId === this.state.userId) {
            return null;
        }

        if (!amISubscribed) {
            return <button className="btn btn-sm btn-outline-primary"
                           onClick={() => this.subscribeRecurrent(eventWeekDayId)}
                           disabled={!this.state.isActive || !haveFreePlaces}>Записаться</button>
        }
        else {
            return <button className="btn btn-sm btn-outline-secondary"
                           onClick={() => this.unsubscribeRecurrent(eventWeekDayId)}
                           disabled={!this.state.isActive}>Отписаться</button>
        }
    }

    renderParticipantsBlock() {
        const tabs = this.state.isRecurrent ?
            <ul className="nav nav-tabs">
                {this.state.weekDays.map(x =>
                    <li key={x.id} className="nav-item">
                        <a type="button" className={`nav-link ${this.state.activeWeekDayId === x.id ? 'active' : null}`}
                           data-id={x.id} onClick={e => this.changeWeekDay(e)}>
                            {DateTimeHelper.daysOfWeek[x.weekDay]} ({x.date})
                        </a>
                    </li>
                )}
            </ul> : null

        const searchBar = null

        const participantsTableRows =
            this.state.participantsList.map((x, index) => {
                return (
                    <tr key={x.id} style={{background: x.isChecked ? 'lightgray' : 'white'}}>
                        <th scope="row">{index + 1}</th>
                        <td><Link to={`/user?id=${x.userId}`}>{x.userName}</Link></td>
                        <td>{x.code}</td>
                        <td><input className="form-check-input ml-0" type="checkbox" checked={x.isChecked}
                                   onChange={() => this.toggleCheck(x.id)}/></td>
                    </tr>
                )
            });

        const table = participantsTableRows.length === 0 ? <div className="m-2">Пока ещё никто не записался</div> :
            <table className="table table-sm m-0">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Имя пользователя</th>
                        <th>Код</th>
                        <th>Присутствует</th>
                    </tr>
                </thead>
                <tbody className="overflow-auto" style={{maxHeight: 400}}>
                    {participantsTableRows}
                </tbody>
            </table>

        return(
            <div className="mt-2">
                {tabs}
                <div className="border">
                    <h5 className="m-2">Участники мероприятия:</h5>
                    {searchBar}
                    {table}
                </div>
                
            </div>
        )
    }

    renderEvent() {
        const image = this.state.image 
            ? <img className="w-100 border" src={this.state.image} alt="event image"/> 
            : null;

        const tags = Object.keys(this.state.tags).length === 0 ? null :
            <div className="p-2">
                {Object.keys(this.state.tags).map((key, index) => {
                    return (
                        <Link type="btn" className="btn btn-sm btn-outline-primary px-1 py-0 mr-2"
                              to={"/events?tag=" + this.state.tags[key]} key={key}>
                            #{this.state.tags[key]}
                        </Link>
                    );
                })}
            </div>

        const start = this.state.isRecurrent ?
            <table className="table table-borderless table-sm m-0 w-auto">
                <tbody>
                    {this.state.weekDays.map(x =>
                        <tr key={x.id}>
                            <td className="p-0">{DateTimeHelper.daysOfWeek[x.weekDay]}</td>
                            <td className="p-0 pl-1">— {x.time}</td>
                        </tr>)}
                </tbody>
            </table> : this.state.start;

        const weekDaysParticipants = this.state.isRecurrent ?
            <table className="table table-borderless table-sm m-0">
                <tbody>
                {this.state.weekDays.map(x =>
                    <tr key={x.id}>
                        <td className="p-0">{DateTimeHelper.daysOfWeek[x.weekDay]}</td>
                        <td className="p-0 pl-1">— {this.getFreePlaces(x.participants)}</td>
                        <td className="p-0 pl-2">
                            {this.renderSubscribeButtonForRecurrent(x.amISubscribed,
                                this.state.participantsLimit === 0 || x.participants < this.state.participantsLimit, x.id)}
                        </td>
                    </tr>)}
                </tbody>
            </table> : <>{this.getFreePlaces(this.state.participants)} <span className="pl-2">{this.renderSubscribeButton()}</span></>;

        return(
            <div className="mx-auto" style={{maxWidth: '720px'}}>
                <div className="list-group mb-3">
                    <div className="list-group-item bg-light d-flex justify-content-between">
                        <h3 className="m-0">{this.state.name}
                            <Link className="h6 ml-2" to={`/events?categoryId=${this.state.categoryId}`}>{this.state.category}</Link>
                        </h3>
                        {this.renderButtonPanel()}
                    </div>
                    <div className="list-group-item">
                        {image}
                        <table className="mt-2" cellPadding="8px">
                            <tbody>
                                <tr>
                                    <td><b>Организатор:</b></td>
                                    <td><Link to={`/user?id=${this.state.organizerId}`}>{this.state.organizer}</Link></td>
                                </tr>
                                <tr>
                                    <td><b>Место проведения:</b></td>
                                    <td>{this.state.place}</td>
                                </tr>
                                <tr>
                                    <td><b>Стоимость:</b></td>
                                    <td>{this.state.fee ? this.state.fee + ' BYN' : 'Бесплатно'}</td>
                                </tr>
                                <tr>
                                    <td className="align-top"><b>{this.state.isRecurrent ? 'Дни проведения:' : 'Начало:'}</b></td>
                                    <td>{start}</td>
                                </tr>
                                <tr className="p-2">
                                    <td className="align-top"><b>Свободно мест:</b></td>
                                    <td>{weekDaysParticipants}</td>
                                </tr>
                            </tbody>
                        </table>
                        <div className="p-2">
                            <b>Описание:</b>
                            <div>
                                { Parser(this.state.description) }
                            </div>
                        </div>
                        {tags}
                        {this.isOrganizerOrAdmin() ? this.renderParticipantsBlock() : null}
                    </div>
                </div>
                <CommentsBlock eventId={this.state.id}/>
            </div>
        ) 
    }

    render() {
        let content;
        switch (this.state.statusCode) {
            case 204:
                content = 
                    <Alert color="info">
                        {"Мероприятие удалено или ещё не создано"}
                    </Alert>
                break;
            case 404:
            case 500:
                content = <ErrorPage code={this.state.statusCode}/>
                break;
            default:
                content = this.renderEvent()
        }

        return (
            this.state.loading ? null : content
        )
    }

    async loadData(eventId) {
        AuthHelper.fetchWithCredentials('api/events/' + eventId)
            .then((response) => {
                this.setState({
                    error: !response.ok,
                    statusCode: response.status
                });
                return response.json();
            }).then((data) => {
                if (this.state.error) {
                    console.log(data);
                }
                else {
                    this.setState({ 
                        id: data.id,
                        name: data.name,
                        categoryId: data.categoryId,
                        category: data.category,
                        description: data.description,
                        start: data.start,
                        startParsable: data.startParsable,
                        place: data.place,
                        fee: data.fee,
                        participantsLimit: data.participantsLimit,
                        organizerId: data.organizerId,
                        organizer: data.organizer,
                        participants: data.participants,
                        tags: data.tags,
                        publicationTime: data.publicationTime,
                        image: data.image,
                        isRecurrent: data.isRecurrent,
                        amISubscribed: data.amISubscribed,
                        isActive: data.isActive,
                        isApproved: data.isApproved,
                        weekDays: data.isRecurrent ? data.weekDays : null,
                        activeWeekDayId: data.isRecurrent ? data.weekDays[0].id : null
                    }, this.loadParticipantsIfNecessary);
                }
            }).catch((ex) => {
                console.log(ex.toString());
            }).finally(() => {
                this.setState({
                    loading: false
                });
            });
    }

    async deleteEvent() {
        AuthHelper.fetchWithCredentials('api/events/' + this.state.id, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.props.history.push("/events");                   
            } 
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async subscribe() {
        AuthHelper.fetchWithCredentials(`api/events/${this.state.id}/subscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadData(this.state.id);
            } 
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async unsubscribe() {
        AuthHelper.fetchWithCredentials(`api/events/${this.state.id}/unsubscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadData(this.state.id);
            } 
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async subscribeRecurrent(eventWeekDayId) {
        AuthHelper.fetchWithCredentials(`api/recurrentEvents/${eventWeekDayId}/subscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadData(this.state.id);
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async unsubscribeRecurrent(eventWeekDayId) {
        AuthHelper.fetchWithCredentials(`api/recurrentEvents/${eventWeekDayId}/unsubscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadData(this.state.id);
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async toggleApprove() {
        AuthHelper.fetchWithCredentials(`api/events/${this.state.id}/approve`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadData(this.state.id);
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }

    async loadParticipants() {
        const path = this.state.isRecurrent
            ? `api/recurrentEvents/${this.state.activeWeekDayId}/participants`
            : `api/events/${this.state.id}/participants`;

        AuthHelper.fetchWithCredentials(path)
            .then((response) => {
                this.setState({
                    error: !response.ok,
                    statusCode: response.status
                });
                return response.json();
            }).then((data) => {
            if (this.state.error) {
                console.log(data);
            }
            else {
                this.setState({
                    participantsList: data
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

    async toggleCheck(participantId) {
        const path = this.state.isRecurrent
            ? `api/recurrentEvents/checkParticipant/${participantId}`
            : `api/events/checkParticipant/${participantId}`;

        AuthHelper.fetchWithCredentials(path, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => {
            if (response.ok) {
                this.loadParticipantsIfNecessary()
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                console.log(response.json());
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}