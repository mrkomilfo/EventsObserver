import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Common/Layout';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Events} />
        <Route path='/event' component={Event} />
        <Route path='/newEvent' component={NewEvent} />
        <Route path='/editEvent' component={EditEvent} />

        <Route path='/users' component={Users} />
        <Route path='/profile' component={Profile} />
        <Route path='/register' component={Register} />
        <Route path='/signIn' component={SignIn} />
        <Route path='/editProfile' component={editProfile} />
        <Route path='/roles' component={Roles} />

        <Route path='/categories' component={Categories} />
        <Route path='/category' component={Category} />
        <Route path='/newCategory' component={NewCategory} />
        <Route path='/editCategory' component={EditCategory} />
      </Layout>
    );
  }
}
