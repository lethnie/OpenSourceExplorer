import React from 'react';
import './header.css';

export default class Header extends React.Component {
    render() {
        return (
            <div className="header-container">
                <p className="header">What kind of an opensource project are you looking for?</p>
            </div>
        );
    }
}
