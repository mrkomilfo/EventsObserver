import React, { Component } from 'react';
import { Route, Switch, Redirect } from 'react-router-dom';
import Layout from './components/Common/Layout';
import Events from './components/Events/Events';
import EventDetail from './components/Events/EventDetail';
import NewEvent from './components/Events/NewEvent';
import EditEvent from './components/Events/EditEvent';
import Profile from './components/Users/Profile';
import SignUp from './components/Users/SignUp';
import SignIn from './components/Users/SignIn';
import EditProfile from './components/Users/EditProfile';
import ChangePassword from './components/Users/ChangePassword';

export default class App extends Component {
  render () {
    return (
        <Layout>
            <Switch>
            <Route path='/events' component={Events} />
            <Route exact path='/' render={() => (<Redirect to='/events' />)} />
            <Route path='/event' component={EventDetail} />
            <Route path='/newEvent' component={NewEvent} />
            <Route path='/editEvent' component={EditEvent} />

            {/*<Route path='/users' component={Users} />*/}
            <Route path='/user' component={Profile} />
            <Route path='/signUp' component={SignUp} />
            <Route path='/signIn' component={SignIn} />
            <Route path='/editProfile' component={EditProfile} />
            <Route path='/changePassword' component={ChangePassword} />
            {/*<Route path='/roles' component={Roles} />
            <Route path='/blocking' component={Blocking} />

            <Route path='/categories' component={Categories} />
            <Route path='/category' component={Category} />
            <Route path='/newCategory' component={NewCategory} />
            <Route path='/editCategory' component={EditCategory} />*/}
            </Switch>
        </Layout>
    );
  }
}
