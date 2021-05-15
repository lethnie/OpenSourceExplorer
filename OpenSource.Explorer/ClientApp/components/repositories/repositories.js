import React from 'react';
import { Alert, Container, Row, Col, Spinner } from 'reactstrap';
import Repository from './repository/repository';
import RepositoriesList from './repositoriesList/repositoriesList';
import Filters from './filters/filters';
import 'isomorphic-fetch';
import { stringify } from 'query-string';
import { PAGE_SIZE } from '../../constants';
import './repositories.css';

export default class Repositories extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            repositoryPage: null,
            repository: null,
            search: null,
            page: { pageSize: PAGE_SIZE, pageNumber: 1 },
            error: null,
            loading: false
        };
        this.handleError = this.handleError.bind(this);
        this.search = this.search.bind(this);
        this.goToPage = this.goToPage.bind(this);
        this.tryYourLuck = this.tryYourLuck.bind(this);
        this.loadRepositories = this.loadRepositories.bind(this);
        this.loadRandomRepository = this.loadRandomRepository.bind(this);
    }

    loadRepositories(search, page) {
        // if search parameters are not set
        if (!search || Object.keys(search).filter(key => search[key]).length === 0) {
            this.setState({ error: 'Need to set search parameters' });
            return;
        }
        // if pagination is not set
        if (!page) {
            this.setState({ error: 'Need to set page parameters' });
            return;
        }
        this.setState({ loading: true });
        let queryParams = Object.assign({}, search);
        queryParams = Object.assign(queryParams, page);
        let query = stringify(queryParams);
        return fetch(constants.searchRepositories + '?' + query)
            .then((response) => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then((data) => {
                data.pageNumber = page.pageNumber;
                this.setState({ repository: null, repositoryPage: data, loading: false, error: null });
            }).catch((err) => {
                this.handleError(err);
            });
    }

    loadRandomRepository(search) {
        // if search parameters are not set
        if (!search || Object.keys(search).filter(key => search[key]).length === 0) {
            this.setState({ error: 'Need to set search parameters' });
            return;
        }
        this.setState({ loading: true });
        let query = stringify(search);
        return fetch(constants.loadRandomRepository + '?' + query)
            .then((response) => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then((data) => {
                this.setState({ repository: { item: data.repository }, repositoryPage: null, loading: false, error: null });
            }).catch((err) => {
                this.handleError(err);
            });
    }

    search(event) {
        let page = { pageSize: PAGE_SIZE, pageNumber: 1 };
        this.setState({ search: event, page: page});
        this.loadRepositories(event, page);
    }

    goToPage(event) {
        this.setState({ page: event });
        this.loadRepositories(this.state.search, event);
    }

    tryYourLuck(event) {
        this.setState({ search: event });
        this.loadRandomRepository(event);
    }

    handleError(err) {
        if (typeof err.json === 'function') {
            err.json().then(error => {
                this.setState({ error: error.message, loading: false });
            });
        } else {
            this.setState({ error: err, loading: false });
        }
    }

    render() {
        let content = this.state.loading ?
            <Spinner className="repositories-spinner" style={{ width: '10rem', height: '10rem' }} /> :
            this.state.repository ?
                this.state.repository.item ?
                    <Repository key={this.state.repository.item.key} item={this.state.repository.item} /> :
                    <div>No results found :(</div> :
                this.state.repositoryPage && this.state.repositoryPage.totalCount > 0 ?
                    <RepositoriesList repositoryPage={this.state.repositoryPage} goToPage={this.goToPage} /> :
                    <div>No results found :(</div>;

        return (
            <Container className="repositories-container">
                <Alert color="danger" isOpen={this.state.error !== null} toggle={() => this.setState({ error: null })}>
                    {this.state.error}
                </Alert>
                <Row>
                    <Col xl="2" md="3" sm="4" xs="12">
                        <Filters className="filters-container" search={this.search} tryYourLuck={this.tryYourLuck} />
                    </Col>
                    <Col xl="8" md="7" sm="8" xs="12">
                        <div className="repositories-content">
                            {content}
                        </div>
                    </Col>
                </Row>
            </Container>
        );
    }
}
