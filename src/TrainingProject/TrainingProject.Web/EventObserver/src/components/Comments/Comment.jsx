import React, { Component } from 'react';

import AuthHelper from "../../Utils/authHelper";

import './Comment.css';

export default class Comment extends Component {
    constructor (props) {
        super(props);
        
        this.state = {
            userRole: AuthHelper.getRole(),
            userId: AuthHelper.getId()
        };

        this.onAuthorDateClick = this.onAuthorDateClick.bind(this);
    }

    onAuthorDateClick() {
        window.location.href = `/user?id=${this.props.comment.author.id}`;
    }

    canIDeleteThisComment() {
        return this.state.userRole === "Admin" || this.state.userId === this.props.comment.author.id;
    }
    
    render() {
        const deleteButton = this.canIDeleteThisComment()
            ? <div className="deleteButton" onClick={() => this.props.onDeleteCommentClick(this.props.comment.id)}>✖</div> 
            : null;

        return(
            <div className="form comment list-group">
                <div className="commentHeader list-group-item">
                    <div className="authorDate" onClick={this.onAuthorDateClick}>
                        <img className="authorIcon" src={this.props.comment.author.photo} alt="author photo"/>
                        <div className="authorDateWrapper">
                            <p className="commentAuthorName">{this.props.comment.author.userName}</p>
                            <p className="commentDate">{this.props.comment.publicationTime}</p>
                        </div>
                    </div>
                    {deleteButton}
                </div>
                <div className="commentBody list-group-item">
                    {this.props.comment.message}
                </div>
            </div>
        )
    }
}