
import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter } from 'react-router-dom';
import * as RoutesModule from './routes';
//import config from './config';

let routes = RoutesModule.routes;
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;

function renderApp() {
    ReactDOM.render(
        <AppContainer>
            <BrowserRouter children={routes} basename={baseUrl} />
        </AppContainer>,
        document.getElementById('root')
    );
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept('./routes', () => {
        routes = require<typeof RoutesModule>('./routes').routes;
        renderApp();
    });
}