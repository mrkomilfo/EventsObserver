import React, { Component } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import {Input, Button} from 'reactstrap';
import AuthHelper from '../../Utils/authHelper.js';
import PropTypes from 'prop-types';
import Draggable from 'react-draggable';
import './Chat.css'

export default class Chat extends Component {
    constructor(props) {
        super(props);
        this.state = {
            message: '',
            messages: [],
            hubConnection: null,
        };
        this.enterPressed = this.enterPressed.bind(this);
        this.sendMessage = this.sendMessage.bind(this);
        this.getAccessToken = this.getAccessToken.bind(this)
    };

    pad = (n, width, z) => {
        z = z || '0';
        n = n + '';
        return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
    };

    componentDidMount() {
        const hubConnection = new HubConnectionBuilder()
            .withUrl('/chat', {
                accessTokenFactory: () => this.getAccessToken()
            })
            .withAutomaticReconnect()
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(() => {
                    console.log('Connection started!')
                }).catch(err => {
                    console.log('Error while establishing connection :(')
                });

            this.state.hubConnection.on('Send', (nick, receivedMessage) => {
                var today = new Date();
                var time = this.pad(today.getHours(), 2) + ":" + this.pad(today.getMinutes(), 2) + ":" + this.pad(today.getSeconds(), 2);
                const message = {
                    time,
                    nick,
                    text: receivedMessage
                }
                const messages = this.state.messages.concat([message]);
                this.setState({ messages });
            });
        });
    };

    componentWillUnmount() {
        this.state.hubConnection.stop()
    }

    sendMessage() {
        if(!this.state.message.trim())
        {
            return;
        }
        this.state.hubConnection
            .invoke('Send', this.props.eventId, this.state.message)
            .catch(err => console.error(err));
  
        this.setState({message: ''});      
    };

    enterPressed(event) {
        var code = event.keyCode || event.which;
        if (code === 13) { 
            this.sendMessage();
        }
    };

    render() {
        return (
            <Draggable handle=".chatHeader">
                <div className="chat">
                    <p className="chatHeader">Чат мероприятия</p>
                    <div id="chatTextBlock" className="chatTextBlock">
                    {this.state.messages.map((message, index) => (
                        <span className="chatMessage" key={index}><b>{`[${message.time}] ${message.nick}: `}</b>{message.text}</span>
                    ))}
                    </div>
                    <div className="chatFooter">
                        <Input className="chatInput" type="text" value={this.state.message} onKeyPress={this.enterPressed} onChange={e => this.setState({ message: e.target.value })}/>        
                        <Button className="chatButton" color="primary" onClick={this.sendMessage}>Send</Button>
                    </div>
                </div>
            </Draggable>
         );
    }

    async getAccessToken() {
        const response = await AuthHelper.fetchWithCredentials(`api/Events/${this.props.eventId}/checkInvolvement`);
        if (response.status === 401) {
            window.location.reload();
        }
        return AuthHelper.getAccessToken();
    }
}

Chat.propTypes = {
    eventId: PropTypes.number.isRequired
};