import React, { Component } from 'react';
import { HubConnectionBuilder } from '@aspnet/signalr';
import AuthHelper from '../../Utils/AuthHelper.js';
import PropTypes from 'prop-types';

export default class Chat extends Component {
    constructor(props) {
        super(props);
        this.state = {
            message: '',
            messages: [],
            hubConnection: null,
        };
    };

    pad = (n, width, z) => {
        z = z || '0';
        n = n + '';
        return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
    };

    componentDidMount = () => {
        const token = AuthHelper.getToken();
        const hubConnection = new HubConnectionBuilder()
            .withUrl('/chat', { accessTokenFactory: () => token })
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(() => console.log('Connection started!'))
                .catch(err => console.log('Error while establishing connection :('));
            var today = new Date();
            var time = this.pad(today.getHours(), 2) + ":" + this.pad(today.getMinutes(), 2) + ":" + this.pad(today.getSeconds(), 2);
            this.state.hubConnection.on('Send', (nick, receivedMessage) => {
                const text = `[${time}] ${nick}: ${receivedMessage}`;
                const messages = this.state.messages.concat([text]);
                this.setState({ messages });
            });
        });
    };

    sendMessage = () => {
        this.state.hubConnection
            .invoke('Send', this.props.eventId, this.state.message)
            .catch(err => console.error(err));
  
        this.setState({message: ''});      
    };

    render() {
        return (
            <div>
                <br />
                <input
                type="text"
                value={this.state.message}
                onChange={e => this.setState({ message: e.target.value })}
                />
    
                <button onClick={this.sendMessage}>Send</button>
    
                <div>
                {this.state.messages.map((message, index) => (
                    <span style={{display: 'block'}} key={index}> {message} </span>
                ))}
                </div>
            </div>
            );
        }
}

Chat.propTypes = {
    eventId: PropTypes.number.isRequired
};