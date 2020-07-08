import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Page404 from './404';
import Page403 from './403';
import Page500 from './500';

export default class ErrorPage extends Component {
    render() {
        switch (this.props.code) {
            case 403: return <Page403/>
            case 404: return <Page404/>
            case 500: return <Page500/>
            default: return null
        }
    }
}

ErrorPage.propTypes = {
    code: PropTypes.number.isRequired
};