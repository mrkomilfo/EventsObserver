import React, { Component } from 'react';
import { Route, Switch, Redirect } from 'react-router-dom';
import Layout from './components/Common/Layout';
import Events from './components/Events/Events';
import EventDetail from './components/Events/EventDetail';
import NewEvent from './components/Events/NewEvent';
import EditEvent from './components/Events/EditEvent';
import Users from './components/Users/Users';
import Profile from './components/Users/Profile';
import SignUp from './components/Users/SignUp';
import SignIn from './components/Users/SignIn';
import EditProfile from './components/Users/EditProfile';
import Blocking from './components/Users/Blocking';
import ChangePassword from './components/Users/ChangePassword';
import Roles from './components/Users/Roles';
import Categories from './components/Categories/Categories';
import CategoryDetail from './components/Categories/CategoryDetail';
import NewCategory from './components/Categories/NewCategory';
import EditCategory from './components/Categories/EditCategory';
import AuthHelper from './Utils/authHelper';
import Error404 from './components/Common/404';
import Error403 from './components/Common/403';

export default class App extends Component {
  render () {
    return (
        <Layout>
            <Switch>
                <Route path='/events' component={Events} />
                <Route exact path='/' render={() => (<Redirect to='/events' />)} />
                <Route path='/event' component={EventDetail} />
                {AuthHelper.getRole() !== 'Guest' ? <Route path='/newEvent' component={NewEvent} /> : <Route path='/newEvent' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? <Route path='/editEvent' component={EditEvent} /> : <Route path='/editEvent' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'User' ? <Route path='/users' component={Error403} /> : <Route path='/users' component={Users} />) : <Route path='/users' render={() => (<Redirect to='/signIn' push/>)} />}
                <Route path='/user' component={Profile} />
                <Route path='/signUp' component={SignUp} />
                {AuthHelper.getRole() === 'Guest' ? <Route path='/signIn' component={SignIn} /> : <Route path='/signIn' render={() => (<Redirect to='/events'/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? <Route path='/editProfile' component={EditProfile} /> : <Route path='/editProfile' render={() => (<Redirect to='/signIn' />)} />}
                {AuthHelper.getRole() !== 'Guest' ? <Route path='/changePassword' component={ChangePassword} /> : <Route path='/changePassword' render={() => (<Redirect to='/signIn' />)} /> }
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'Account manager' ? <Route path='/roles' component={Roles} /> : <Route path='/roles' component={Error403} />) : <Route path='/roles' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'User' ? <Route path='/blocking' component={Error403} /> : <Route path='/blocking' component={Blocking} />) : <Route path='/blocking' render={() => (<Redirect to='/signIn' push/>)} />}

                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'Admin' ? <Route path='/categories' component={Categories} /> : <Route path='/categories' component={Error403} />) : <Route path='/categories' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'Admin' ? <Route path='/category' component={CategoryDetail} /> : <Route path='/category' component={Error403} />) : <Route path='/category' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'Admin' ? <Route path='/newCategory' component={NewCategory} /> : <Route path='/newCategory' component={Error403} />) : <Route path='/newCategory' render={() => (<Redirect to='/signIn' push/>)} />}
                {AuthHelper.getRole() !== 'Guest' ? (AuthHelper.getRole() === 'Admin' ? <Route path='/editCategory' component={EditCategory} /> : <Route path='/categories' component={Error403} />) : <Route path='/editCategory' render={() => (<Redirect to='/signIn' push/>)} />}
                
                <Route path="*" component={Error404}/>
            </Switch>
        </Layout>
    );
  }
}
