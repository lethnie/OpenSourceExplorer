import React from 'react';
import { Badge } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBug, faCodeBranch, faCalendarCheck, faHandsHelping, faStar } from '@fortawesome/free-solid-svg-icons';
import './repository.css';

export default class Repository extends React.Component {
    render() {
        let item = this.props.item;
        if (!item) {
            return <div />;
        }
        let languages = item.languages.map(lang => {
            return (
                <Badge className="repository-language" key={lang}>{lang}</Badge>
            );
        });
        if (item.languagesTotalCount > item.languages.length) {
            languages.push(<Badge className="repository-language" key="..." href={item.url} target="_blank" rel="noopener noreferrer">...</Badge>);
        }
        let helpWanted = item.helpWantedIssuesCount > 0 ?
            (<div className="repository-stats-item" title={`${item.helpWantedIssuesCount} issues`}>
                <FontAwesomeIcon icon={faHandsHelping} className="repository-icon" />help wanted
            </div>) : <div style={{ display: 'inline' }} />;
        let goodFirstIssue = item.goodFirstIssuesCount > 0 ?
            (<div className="repository-stats-item" title={`${item.goodFirstIssuesCount} issues`}>
                <FontAwesomeIcon icon={faBug} className="repository-icon" />good first issue
            </div>) : <div style={{ display: 'inline' }} />;
        return (
            <div className="repository-item" key={item.key}>
                <div className="repository-title">
                    <a href={item.url} target="_blank" rel="noopener noreferrer">{item.owner}/{item.name}</a>
                </div>
                <div className="repository-description">{item.description}</div>
                <div className="repository-stats">
                    <div className="repository-stats-item" title="Number of stars">
                        <FontAwesomeIcon icon={faStar} className="repository-icon" />{item.starsCount}
                    </div>
                    <div className="repository-stats-item" title="Number of forks">
                        <FontAwesomeIcon icon={faCodeBranch} className="repository-icon" />{item.forksCount}
                    </div>
                    <div className="repository-stats-item" title={`Last update: ${new Date(item.updatedDate).toLocaleString()}`}>
                        <FontAwesomeIcon icon={faCalendarCheck} className="repository-icon" />
                        {new Date(item.updatedDate).toLocaleDateString()}
                    </div>
                    {helpWanted}
                    {goodFirstIssue}
                </div>
                <div>{languages}</div>
            </div>
        );
    }
}
