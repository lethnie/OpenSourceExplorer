import React from 'react';
import { Pagination, PaginationItem, PaginationLink, Spinner } from 'reactstrap';
import Repository from '../repository/repository';
import { PAGE_SIZE } from '../../../constants';
import './repositoriesList.css';

export default class RepositoriesList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            pageNumber: 1,
            pageSize: PAGE_SIZE
        };
        this.goToPage = this.goToPage.bind(this);
    }

    goToPage = (e, index) => {
        e.preventDefault();

        if (!this.props.repositoryPage) {
            return;
        }

        let pageCount = Math.ceil(this.props.repositoryPage.totalCount / this.state.pageSize);
        if (index < 1 || index > pageCount) {
            return;
        }

        this.setState({
            pageNumber: index
        });

        this.props.goToPage({
            pageNumber: index,
            pageSize: this.state.pageSize
        });

        // scroll to the beginning of the list
        window.scrollTo(0, 0);
    };

    render() {
        let repositories = this.props.loading ?
            <Spinner className="repositories-spinner" style={{ width: '10rem', height: '10rem' }} /> :
            this.props.repositoryPage ?
                this.props.repositoryPage.repositories.map(item => {
                    return (
                        <Repository key={item.key} item={item} />
                    );
                }) : <div />;
        let pagination = <div />;
        if (this.props.repositoryPage) {
            let pageNumbers = [];
            let pageCount = Math.ceil(this.props.repositoryPage.totalCount / this.state.pageSize);
            let currentPage = this.state.pageNumber;
            const maxShowPages = 3; // on both sides from current
            const approxPageNumWidth = 50;
            const defaultButtonsCount = 7; // prev, next, first, last, + two breaks (...)
            // calculate number of pages to show on both sides from current page
            let showPages = Math.max(0,
                Math.min(maxShowPages, Math.floor((window.outerWidth / approxPageNumWidth - defaultButtonsCount) / 2)));
            if (pageCount > 1) {
                pageNumbers.push(<PaginationItem key='prevPage'>
                    <PaginationLink previous onClick={e => this.goToPage(e, currentPage - 1)} href="#" />
                </PaginationItem>);
                // calculate first displayed page in the range
                let index = currentPage - showPages;
                let count = showPages * 2 + 1;
                if (index < 1) {
                    count = count - (1 - index);
                    index = 1;
                }
                // calculate number of displayed pages in the range
                if (currentPage + showPages > pageCount) {
                    count = count - (currentPage + showPages - pageCount);
                }
                if (index > 1) {
                    pageNumbers.push(
                        <PaginationItem key="page1" active={currentPage === 1 ? true : false}>
                            <PaginationLink onClick={e => this.goToPage(e, 1)} href="#">1</PaginationLink>
                        </PaginationItem>
                    );
                    if (index > 2) {
                        pageNumbers.push(<div key="break1" className="page-break">...</div>);
                    }
                }
                for (let i = index; i < index + count; i++) {
                    pageNumbers.push(
                        <PaginationItem key={'page' + i.toString()} active={currentPage === i ? true : false}>
                            <PaginationLink onClick={e => this.goToPage(e, i)} href="#">
                                {i}
                            </PaginationLink>
                        </PaginationItem>
                    );
                }
                if (index + count <= pageCount) {
                    if (index + count <= pageCount - 1) {
                        pageNumbers.push(<div key="break2" className="page-break">...</div>);
                    }
                    pageNumbers.push(
                        <PaginationItem key={'page' + pageCount.toString()} active={currentPage === pageCount ? true : false}>
                            <PaginationLink onClick={e => this.goToPage(e, pageCount)} href="#">
                                {pageCount}
                            </PaginationLink>
                        </PaginationItem>
                    );
                }
                pageNumbers.push(<PaginationItem key='nextPage'>
                    <PaginationLink next onClick={e => this.goToPage(e, currentPage + 1)} href="#" />
                </PaginationItem>);

                pagination = <Pagination>{pageNumbers}</Pagination>;
            }
        }
        return (
            <div id="repositories" className="repositories-content">
                {repositories}
                {pagination}
            </div>
        );
    }
}
