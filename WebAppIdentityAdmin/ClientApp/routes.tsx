import * as React from 'react';
import { Route, Switch } from 'react-router-dom';
import { Layout } from './layout/Layout';
import Home from './screens/home';
import * as Error from './screens/errors';
export const routes =
    <Layout>
        <Switch>
            <Route exact path='/' component={Home} />
            <Route component={Error.Error404} />
        </Switch>
    </Layout>;