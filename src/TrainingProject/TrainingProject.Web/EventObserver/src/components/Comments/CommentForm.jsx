import React, { Component } from 'react';
import AuthHelper from "../../Utils/authHelper";

export default class CommentForm extends Component {
    constructor (props) {
        super(props);

        this.state = {
            comment: '',
        };

        this.sendComment = this.sendComment.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    handleInputChange(event) {
        this.setState({
            comment: event.target.value
        })
    }

    render() {
        const textAreaStyle = {
            minHeight: 40,
            maxHeight: 150
        }

        return(
            <div className="input-group mb-3">
                <textarea className="form-control" style={textAreaStyle} placeholder="Ваш комментарий..."
                    value={this.state.comment} onChange={this.handleInputChange}/>
                <div className="input-group-append">
                    <button type="button" disabled={!this.state.comment} className="btn btn-primary"
                            onClick={this.sendComment}>Отправить</button>
                </div>
            </div>
        )
    }
    
    sendComment() {
        const data = {
            eventId: this.props.eventId,
            authorId: AuthHelper.getId(),
            message: this.state.comment
        }

        AuthHelper.fetchWithCredentials('api/comments', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(data)
        }).then((response) => {
            if (response.ok){
                this.props.loadComments();
            }
            else if (response.status === 401) {
                this.props.history.push("/signIn");
            }
            else {
                return response.json();
            }
        }).then((data) => {
            if(this.state.error) {
                console.log(data);
            }
        }).catch((ex) => {
            console.log(ex.toString());
        });
    }
}