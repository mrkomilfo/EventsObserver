import React, { Component } from 'react';
import queryString from 'query-string';
import { Alert, Table, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Link } from 'react-router-dom';
import AuthHelper from '../../Utils/AuthHelper.js';

export default class EventDetail extends Component {
    constructor(props)
    {
        super(props);
        this.state = {
            loading: true,
            error: false,
            errorMessage: '',
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
            userId: AuthHelper.getId()
        }
        this.deleteEvent = this.deleteEvent.bind(this);
        this.subscribe = this.subscribe.bind(this);
        this.unsubscribe = this.unsubscribe.bind(this);
        this.toggleDeleteModal = this.toggleDeleteModal.bind(this);
    }

    componentDidMount() {
        const parsed = queryString.parse(window.location.search);
        if (parsed) {
            this.loadData(parsed['id']);
        }
    }

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    renderBottomButtonPanel()
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

        if (this.state.userId === this.state.organizerId || this.state.userRole == 'Admin')
        {
            return(
                <div>
                    <Button outline color="primary" tag={Link} to={`/editEvent?id=${this.state.id}`}>Редактировать</Button>{' '}
                    <Button color="danger" onClick={this.toggleDeleteModal}>Удалить</Button>
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
        if (this.state.userRole === 'Guest' || this.state.userId === this.state.organizerId || start <= currentDate)
        {
            return null
        } 
        else if (Object.keys(this.state.participants).includes(this.state.userId)){
            return(
                <Button color="secondary" onClick={this.unsubscribe}>Отписаться</Button>
            )
        }
        else{
            return(
                <Button color="primary" onClick={this.subscribe}>Записаться</Button>
            )
        }
    }

    renderEvent()
    {
        const inlineStyle = {display: 'table'};
        const inlineFirst = {display: 'table-cell'};
        const inlineSecond = {marginLeft: '12px'};
        const imageStyle = {width: '100%'};
        const tagBlockStyle = {display: 'inline'};
        const subscribeButtonStyle = {margin: '0px 0px 12px 8px', position: 'relative', top: '-2px'}

        const image = this.state.image 
            ? <img style={imageStyle} src={this.state.image} alt="event image"/> 
            : null;

        const tags = Object.keys(this.state.tags).map((key, index) => {
            return (
                <Link className="link" to={"/events?tag=" + this.state.tags[key]} key={key}>#{this.state.tags[key]} </Link>
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

        const subscribeButton = this.renderSubscribeButton();

        const bottomButtonPanel = this.renderBottomButtonPanel();

        return(
            <>
                <div style={inlineStyle}><h2 style={inlineFirst}>{this.state.name}</h2><p style={inlineSecond}>Опубликовано: {this.state.publicationTime}</p></div>
                <h4>Категория: <Link to={`/events?categoryId=${this.state.categoryId}`}>{this.state.category}</Link></h4>
                <div style={tagBlockStyle}>{tags}</div>
                {image}
                <p>{this.state.description}</p>
                <table cellPadding='8px'>
                    <tbody>
                    <tr><td><b>Организатор:</b></td><td><Link to={`/user?id=${this.state.organizerId}`}>{this.state.organizer}</Link></td></tr>
                    <tr><td><b>Место проведения:</b></td><td>{this.state.place}</td></tr>
                    <tr><td><b>Начало:</b></td><td>{this.state.start}</td></tr>
                    <tr><td><b>Взнос:</b></td><td>{this.state.fee ? this.state.fee +'BYN' : 'Бесплатно'}</td></tr>
                    </tbody>
                </table>
                <br/>
                <div style={inlineStyle}>
                    <h4 style={inlineFirst}>Записались - {this.state.participantsLimit
                    ? `(${Object.keys(this.state.participants).length}/${this.state.participantsLimit})`
                    : Object.keys(this.state.participants).length}</h4>
                    <div style={subscribeButtonStyle}>{subscribeButton}</div>
                </div>
                <Table striped>
                    <tbody>
                        {participantsTableRows}
                    </tbody>
                </Table>
                {bottomButtonPanel}
            </>
        ) 
    }

    render()
    {
        const errorBaner = this.state.errorMessage ? 
        <Alert color="danger">
            {this.state.errorMessage}
        </Alert> : null;

        const content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEvent();

        return(
            <>
                {errorBaner}
                {content}
            </>
        )
    }

    async loadData(eventId) {
        fetch('api/Events/' + eventId)
        .then((response) => {
            this.setState({error: !response.ok});
            return response.json();
        }).then((data) => {
            if (this.state.error){
                this.setState({ 
                    errorMessage: data.message 
                });
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
                    loading: false
                });
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            });
        });
    }

    async deleteEvent() {
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch('api/Events/' + this.state.id, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            if (response.ok) {
                this.props.history.push("/events");                   
            } 
            else {
                this.setState({error: true})
                return response.json()
            }
        }).then((data) => {
            if (this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                })
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            })
        });
    }

    async subscribe() {
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch(`api/Events/${this.state.id}/subscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            if (response.ok) {
                window.location.reload();    
            } 
            else {
                this.setState({error: true})
                return response.json()
            }
        }).then((data) => {
            if (this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                })
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            })
        });
    }

    async unsubscribe() {
        const token = await AuthHelper.getToken();
        if (!token) {
            this.props.history.push("/signIn");
        }
        fetch(`api/Events/${this.state.id}/unsubscribe`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            }
        }).then((response) => {
            if (response.ok) {
                window.location.reload();             
            } 
            else {
                this.setState({error: true})
                return response.json()
            }
        }).then((data) => {
            if (this.state.error)
            {
                this.setState({
                    errorMessage: data.message
                })
            }
        }).catch((ex) => {
            this.setState({
                errorMessage: ex.toString()
            })
        });
    }
}