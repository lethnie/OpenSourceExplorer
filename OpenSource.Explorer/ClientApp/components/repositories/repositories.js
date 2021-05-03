import React from 'react';
import { Alert, Container, Row, Col, Spinner } from 'reactstrap';
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
            search: null,
            page: { pageSize: PAGE_SIZE, pageNumber: 1 },
            error: null,
            loading: false
        };
        this.search = this.search.bind(this);
        this.goToPage = this.goToPage.bind(this);
    }

    loadRepositories(search, page) {
        // if search parameters are not set
        if (!search || Object.keys(search).filter(key => search[key] !== undefined).length === 0) {
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
                this.setState({ repositoryPage: data, loading: false });
            }).catch((err) => {
                if (typeof err.json === 'function') {
                    err.json().then(error => {
                        this.setState({ error: error.message, loading: false });
                    });
                } else {
                    this.setState({ error: err, loading: false });
                }
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

    render() {
        return (
            <Container className="repositories-container">
                <Alert color="danger" isOpen={this.state.error !== null} toggle={() => this.setState({ error: null })}>
                    {this.state.error}
                </Alert>
                <Row>
                    <Col xl="2" md="3" sm="4" xs="12">
                        <Filters className="filters-container" search={this.search} />
                    </Col>
                    <Col xl="8" md="7" sm="8" xs="12">
                        <RepositoriesList repositoryPage={this.state.repositoryPage} goToPage={this.goToPage} loading={this.state.loading} />
                    </Col>
                </Row>
            </Container>
        );
    }
}
