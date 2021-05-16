import React from 'react';
import { Button, Form, FormGroup, Input, Label } from 'reactstrap';
import SelectSearch from 'react-select-search';
import DayPickerInput from 'react-day-picker/DayPickerInput';
import { formatDate, parseDate } from 'react-day-picker/moment';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faDice } from '@fortawesome/free-solid-svg-icons';
import languages from './languages';
import 'react-day-picker/lib/style.css';
import './filters.css';
import { DATE_FORMAT } from '../../../constants';

export default class Filters extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            text: undefined,
            language: undefined,
            hasGoodFirstIssues: true,
            hasHelpWantedIssues: true,
            minNumberOfStars: undefined,
            lastUpdateAfter: undefined
        };
        this.getFilters = this.getFilters.bind(this);
        this.search = this.search.bind(this);
        this.tryYourLuck = this.tryYourLuck.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleLanguageChange = this.handleLanguageChange.bind(this);
        this.handleLastUpdateAfterChange = this.handleLastUpdateAfterChange.bind(this);
    }

    componentDidMount() {
        this.search();
    }

    search() {
        this.props.search(this.getFilters());
    }

    tryYourLuck() {
        this.props.tryYourLuck(this.getFilters());
    }

    getFilters() {
        return {
            text: this.state.text,
            language: this.state.language,
            hasGoodFirstIssues: this.state.hasGoodFirstIssues,
            hasHelpWantedIssues: this.state.hasHelpWantedIssues,
            minNumberOfStars: this.state.minNumberOfStars,
            lastUpdateAfter: this.state.lastUpdateAfter
        };
    }

    handleInputChange(event) {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }

    handleLanguageChange(value) {
        this.setState({
            language: value === '' ? null : value
        });
    }

    handleLastUpdateAfterChange(value) {
        this.setState({
            lastUpdateAfter: value ? formatDate(value, DATE_FORMAT) : value
        });
    }

    filter(options) {
        return (value) => {
            if (!value.length) {
                return options;
            }

            return options.filter(val => val.value.toLowerCase().indexOf(value.toLowerCase()) >= 0);
        };
    }

    render() {
        let options = languages.map(l => { return { name: l, value: l }; });
        // add empty value to remove selection
        options.unshift({ name: '', value: '' });
        return (
            <Form className={this.props.className}>
                <FormGroup>
                    <Label for="language">Programming Language</Label>
                    <SelectSearch
                        id="language"
                        options={options}
                        search
                        filterOptions={this.filter}
                        emptyMessage="Not found"
                        value={this.state.language}
                        onChange={this.handleLanguageChange}
                        placeholder="C#, Java..."
                    />
                </FormGroup>
                <FormGroup check>
                    <Label check>
                        <Input
                            type="checkbox"
                            name="hasGoodFirstIssues"
                            id="hasGoodFirstIssues"
                            checked={this.state.hasGoodFirstIssues}
                            onChange={this.handleInputChange}
                        />Only With Good First Issues
                    </Label>
                </FormGroup>
                <FormGroup check>
                    <Label check>
                        <Input
                            type="checkbox"
                            name="hasHelpWantedIssues"
                            id="hasHelpWantedIssues"
                            checked={this.state.hasHelpWantedIssues}
                            onChange={this.handleInputChange}
                        />Only With 'Help Wanted' Tags
                    </Label>
                </FormGroup>
                <FormGroup>
                    <Label for="text">Text To Look For</Label>
                    <Input type="text" name="text" id="text"
                        value={this.state.text}
                        onChange={this.handleInputChange}
                        placeholder="Cool Project"
                    />
                </FormGroup>
                <FormGroup>
                    <Label for="minNumberOfStars">Min Number Of Stars</Label>
                    <Input
                        type="number"
                        name="minNumberOfStars"
                        id="minNumberOfStars"
                        value={this.state.minNumberOfStars}
                        onChange={this.handleInputChange}
                        max={new Date()}
                        placeholder="0"
                    />
                </FormGroup>
                <FormGroup>
                    <Label for="lastUpdateDate">Updated After</Label>
                    <DayPickerInput
                        name="lastUpdateAfter"
                        id="lastUpdateAfter"
                        format={DATE_FORMAT}
                        formatDate={formatDate}
                        parseDate={parseDate}
                        placeholder={DATE_FORMAT.toUpperCase()}
                        value={this.state.lastUpdateAfter}
                        onDayChange={this.handleLastUpdateAfterChange}
                        dayPickerProps={{
                            disabledDays: {
                                after: new Date()
                            },
                            modifiersStyles: {
                                today: {
                                    color: 'rgb(147, 149, 151)'
                                },
                                selected: {
                                    backgroundColor: 'rgba(147, 149, 151, 0.5)'
                                }
                            }
                        }}
                    />
                </FormGroup>
                <div className="btn-container">
                    <Button onClick={this.search} className="btn-search">Search</Button>
                    <Button onClick={this.tryYourLuck} className="btn-search" title="Try your luck!">
                        <FontAwesomeIcon icon={faDice} className="btn-icon" />
                    </Button>
                </div>
            </Form>
        );
    }
}
