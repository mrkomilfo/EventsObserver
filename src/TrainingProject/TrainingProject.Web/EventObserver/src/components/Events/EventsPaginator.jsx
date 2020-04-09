import React from 'react';
import { Pagination, PaginationItem, PaginationLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import queryString from 'query-string';
import PropTypes from 'prop-types';

export class EventsPaginator extends Component {
    render() {
        let pagePath = '/events?page='

        let params = queryString.parse(location.search);
        let queryTrailer = '';
        if (params.pageSize){
            queryTrailer += `&pageSize=${params.pageSize}`
        }
        if (params.search){
            queryTrailer += `&search=${params.search}`
        }
        if (params.categoryId){
            queryTrailer += `&categoryId=${params.categoryId}`
        }
        if (params.tag){
            queryTrailer += `&tag=${params.tag}`
        }
        if (params.upComing){
            queryTrailer += `&upComing=${params.upComing}`
        }
        if (params.onlyFree){
            queryTrailer += `&onlyFree=${params.onlyFree}`
        }
        if (params.vacancies){
            queryTrailer += `&vacancies=${params.vacancies}`
        }
        if (params.organizer){
            queryTrailer += `&organizer=${params.organizer}`
        }
        if (params.participant){
            queryTrailer += `&participant=${params.participant}`
        }

        let leftArrows = this.props.currentPage == 0 ? 
        <>
            <PaginationItem disabled>
                <PaginationLink first href="#" />
            </PaginationItem>
            <PaginationItem disabled>
                <PaginationLink previous href="#" />
            </PaginationItem>
        </> :
        <>
            <PaginationItem>
                <PaginationLink first tag={Link} to={`${pagePath}${0}${queryTrailer}`} />
            </PaginationItem>
            <PaginationItem>
                <PaginationLink previous tag={Link} to={`${pagePath}${this.props.currentPage - 1}${queryTrailer}`} />
            </PaginationItem>
        </>;
        let prevprev = this.props.currentPage - 2 >= 0 ?
        <PaginationItem>
            <PaginationLink tag={Link} to={`${pagePath}${this.props.currentPage - 2}${queryTrailer}`}>
            {this.props.currentPage-1}
            </PaginationLink>
         </PaginationItem> : null;
        let prev = this.props.currentPage - 1 >= 0 ?
        <PaginationItem>
            <PaginationLink tag={Link} to={`${pagePath}${this.props.currentPage - 1}${queryTrailer}`}>
            {this.props.currentPage}
            </PaginationLink>
         </PaginationItem> : null;
        let current = 
        <PaginationItem active>
            <PaginationLink tag={Link} to={`${pagePath}${this.props.currentPage}${queryTrailer}`}>
            {this.props.currentPage + 1}
            </PaginationLink>
         </PaginationItem>;
         let next = this.props.currentPage + 2 <= this.props.pages ?
         <PaginationItem>
             <PaginationLink tag={Link} to={`${pagePath}${this.props.currentPage + 1}${queryTrailer}`}>
             {this.props.currentPage + 2}
             </PaginationLink>
         </PaginationItem> : null;
        let nextnext = this.props.currentPage + 3 <= this.props.pages ?
        <PaginationItem>
            <PaginationLink tag={Link} to={`${pagePath}${this.props.currentPage + 2}${queryTrailer}`}>
            {this.props.currentPage + 3}
            </PaginationLink>
        </PaginationItem> : null;
        let rightArrows = this.props.currentPage >= this.props.pages-1 ? 
        <>
            <PaginationItem disabled>
                <PaginationLink first href="#" />
            </PaginationItem>
            <PaginationItem disabled>
                <PaginationLink previous href="#" />
            </PaginationItem>
        </> :
        <>
            <PaginationItem>
                <PaginationLink next tag={Link} to={`${pagePath}${this.props.currentPage + 1}${queryTrailer}`} />
            </PaginationItem>
            <PaginationItem>
                <PaginationLink last tag={Link} to={`${pagePath}${this.props.pages - 1}${queryTrailer}`} />
            </PaginationItem>
        </>;
        return (
            <Pagination aria-label="Events paginator">
                {leftArrows}
                {prevprev}
                {prev}
                {current}
                {next}
                {nextnext}
                {rightArrows}
            </Pagination>
            )
    }
}

EventPaginator.propTypes = {
    currentPage: PropTypes.number.isRequired,
    pages: PropTypes.number.isRequired
};