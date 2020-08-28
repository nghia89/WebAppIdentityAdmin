import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './layout/Layout';
import Home  from './screens/home';
import * as Error from './screens/errors';
export const routes =
    <Layout>
        <Route exact path='/' component={Home} />
        <Route component={Error.Error404} />
    </Layout>;