import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Table, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Link } from 'react-router-dom';
import Parser from 'html-react-parser';
import Chat from './Chat';
import AuthHelper from '../../Utils/authHelper.js';
import ErrorPage from '../Common/ErrorPage';
import CommentsBlock from "../Comments/CommentsBlock";

export default class EventDetail extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,

            error: false,
            statusCode: 200,

            id: null,
            name: '',
            categoryId: null,
            category: '',
            description: '',
            start: '',
            startParsable: 0,
            place: '',
            fee: 0,
            participantsLimit: 0,
            organizerId: '',
            organizer: '',
            participants: [],
            tags: [],
            publicationTime: '',
            image: '',

            deleteModal: false,

            userRole: AuthHelper.getRole(),
            userId: AuthHelper.getId(),
        }
        this.deleteEvent = this.deleteEvent.bind(this);
        this.subscribe = this.subscribe.bind(this);
        this.unsubscribe = this.unsubscribe.bind(this);
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

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    renderButtonPanel()
    {
        const deleteModal = 
            <Modal isOpen={this.state.deleteModal} toggle={this.toggleDeleteModal}>
                <ModalHeader toggle={this.toggleDeleteModal}>Подтвердите действие</ModalHeader>
                <ModalBody>
                    Вы действительно хотите удалить данное событие?
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.deleteEvent}>Да</Button>{' '}
                    <Button color="secondary" onClick={this.toggleDeleteModal}>Отмена</Button>
                </ModalFooter>
            </Modal>

        if (this.state.userId === this.state.organizerId || this.state.userRole === 'Admin')
        {
            return(
                <div style={{minWidth: "fit-content"}}>
                    <Button outline color="primary" tag={Link} to={`/editEvent?id=${this.state.id}`}>
                        <i className="bi bi-pencil"/>
                    </Button>{' '}
                    <Button outline color="danger" onClick={this.toggleDeleteModal}>
                        <i className="bi bi-trash"/>
                    </Button>
                    {deleteModal}
                </div>
            )
        }
        else if (this.state.userRole === 'Admin')
        {
            return(
                <div>
                    <Button color="danger" onClick={this.toggleDeleteModal}>Удалить</Button>
                    {deleteModal}
                </div>
            )
        }
        else {
            return null
        }
    }

    renderSubscribeButton()
    {
        const start = new Date(this.state.startParsable);
        const currentDate = new Date();
        if (Object.keys(this.state.participants).includes(this.state.userId)) {
            return(
                <Button color="secondary" onClick={this.unsubscribe}>Отписаться</Button>
            )
        }
        else if (this.state.userRole === 'Guest' 
            || this.state.userId === this.state.organizerId 
            || start <= currentDate
            || Object.keys(this.state.participants).length >= this.state.participantsLimit && this.state.participantsLimit !== 0) {
            return null
        } 
        else {
            return(
                <Button color="primary" onClick={this.subscribe}>Записаться</Button>
            )
        }
    }

    renderEvent()
    {
        const inlineStyle = {display: 'table'};
        const inlineFirst = {display: 'table-cell'};
        const subscribeButtonStyle = {marginLeft: '8px'}

        const image = this.state.image 
            ? <img className="w-100 border" src={this.state.image} alt="event image"/> 
            : null;

        const tags = Object.keys(this.state.tags).map((key, index) => {
            return (
                <Link type="btn" className="btn btn-sm btn-outline-primary px-1 py-0 mr-2" to={"/events?tag=" + this.state.tags[key]} key={key}>
                    #{this.state.tags[key]}
                </Link>
            );
        });

        const participantsTableRows = Object.keys(this.state.participants).map((key, index) => {
            return (
                <tr key={key}> 
                    <th scope="row">{index+1}</th>
                    <td><Link to={`/user?id=${key}`}>{this.state.participants[key]}</Link></td>
                </tr>
            )
        });

        /*const chat = this.state.userId === this.state.organizerId || Object.keys(this.state.participants).includes(this.state.userId)
            ? <Chat eventId={this.state.id} />
            : null;*/

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
                        <table className="mt-2" cellPadding='8px'>
                            <tbody>
                                <tr>
                                    <td><b>Организатор:</b></td>
                                    <td><Link to={`/user?id=${this.state.organizerId}`}>{this.state.organizer}</Link></td>
                                </tr>
                                <tr><td><b>Место проведения:</b></td><td>{this.state.place}</td></tr>
                                <tr><td><b>Начало:</b></td><td>{this.state.start}</td></tr>
                                <tr><td><b>Стоимость:</b></td><td>{this.state.fee ? this.state.fee +' BYN' : 'Бесплатно'}</td></tr>
                            </tbody>
                        </table>
                        <div className="p-2">
                            <b>Описание:</b>
                            <div>
                                { Parser(this.state.description) }
                            </div>
                        </div>
                        <div className="p-2">{tags}</div>
                        <hr/>
                        <div style={{...inlineStyle, marginBottom: '4px'}}>
                            <h4 style={inlineFirst}>Записались - {this.state.participantsLimit
                            ? `(${Object.keys(this.state.participants).length}/${this.state.participantsLimit})`
                            : Object.keys(this.state.participants).length}</h4>
                            <div style={subscribeButtonStyle}>{this.renderSubscribeButton()}</div>
                        </div>
                        <Table striped>
                            <tbody>
                                {participantsTableRows}
                            </tbody>
                        </Table>
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
            <>
                {this.state.loading ? null : content}
            </>
        )
    }

    async loadData(eventId) {
        await fetch('api/events/' + eventId)
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
                window.location.reload();    
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
                window.location.reload();             
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