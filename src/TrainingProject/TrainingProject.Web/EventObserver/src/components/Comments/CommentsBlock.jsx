import React, { Component } from 'react';
import {Button, Modal, ModalBody, ModalFooter, ModalHeader} from 'reactstrap';

import CommentForm from './CommentForm';
import Comment from './Comment';

import AuthHelper from '../../Utils/authHelper';

export default class CommentsBlock extends Component {
    constructor(props) {
        super(props);
        this.state = {
            error: false,
            loading: true,

            message: '',
            commentToDeleteId: null,

            currentPage: 0,
            pageSize: 8,
            totalRecords: 0,
            comments: [],

            deleteModal: false,
        };

        this.loadComments = this.loadComments.bind(this);
        this.toggleDeleteModal = this.toggleDeleteModal.bind(this);
        this.onDeleteCommentClick = this.onDeleteCommentClick.bind(this);
        this.deleteComment = this.deleteComment.bind(this);
    }

    componentDidMount() {
        this.loadComments();
    }

    toggleDeleteModal(){
        this.setState({
            deleteModal: !this.state.deleteModal
        });
    }

    onDeleteCommentClick(id) {
        this.setState({commentToDeleteId: id});
        this.toggleDeleteModal();
    }

    renderDeleteCommentModal() {
        return(
            <Modal isOpen={this.state.deleteModal} toggle={this.toggleDeleteModal}>
                <ModalHeader toggle={this.toggleDeleteModal}>Подтвердите действие</ModalHeader>
                <ModalBody>
                    Вы действительно хотите удалить данный комментарий?
                </ModalBody>
                <ModalFooter>
                    <Button color="primary" onClick={this.deleteComment}>Да</Button>{' '}
                    <Button color="secondary" onClick={this.toggleDeleteModal}>Отмена</Button>
                </ModalFooter>
            </Modal>
        )
    }

    render() {
        return(
            <div className="list-group">
                <div className="list-group-item bg-light">
                    <h3 className="m-0">Комментарии <span className="h5 text-secondary">({this.state.totalRecords})</span></h3>
                </div>
                <div className="list-group-item">
                    {AuthHelper.getRole() === "Guest" ? null : 
                        <CommentForm eventId={this.props.eventId} loadComments={this.loadComments}/>}
                    {
                        this.state.loading ? null :
                            <div>
                                {this.state.comments.map(comment =>
                                    <Comment key={comment.id} comment={comment} onDeleteCommentClick={this.onDeleteCommentClick}/>)}
                            </div>
                    }
                </div>
                {this.renderDeleteCommentModal()}
            </div>
        )
    }

    loadComments() {
        AuthHelper.fetchWithCredentials(`api/comments/${this.props.eventId}?index=${this.state.currentPage}&pageSize=${this.state.pageSize}`)
            .then((response) => {
                this.setState({
                    error: !response.ok
                });
                return response.json();
            }).then((data) => {
                if (this.state.error) {
                    console.log(data);
                }
                else {
                    this.setState({
                        comments: data.records,
                        currentPage: data.currentPage,
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

    deleteComment() {
        AuthHelper.fetchWithCredentials('api/comments/' + this.state.commentToDeleteId, {
            method: 'DELETE'
        }).then((response) => {
            if (response.ok) {
                this.toggleDeleteModal();
                this.setState({currentPage: 0}, this.loadComments)
            }
        });
    }
}