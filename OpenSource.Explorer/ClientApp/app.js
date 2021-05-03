import React from 'react';
import Header from './components/header/header';
import Repositories from './components/repositories/repositories';
import { BrowserRouter } from 'react-router-dom';

export default class App extends React.Component {
    render() {
        return (
            <BrowserRouter>
                <div>
                    <Header />
                    <Repositories />
                </div>
            </BrowserRouter>
        );
    }
}
