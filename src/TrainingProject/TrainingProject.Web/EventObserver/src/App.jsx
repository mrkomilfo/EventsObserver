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
import Categories from './components/Categories/Categories';
import CategoryDetail from './components/Categories/CategoryDetail';
import NewCategory from './components/Categories/NewCategory';
import EditCategory from './components/Categories/EditCategory';
import AuthHelper from './Utils/authHelper';
import Error404 from './components/Common/404';

export default class App extends Component {
  render () {
    return (
        <Layout>
            <Switch>
              <Route path='/events' component={Events} />
              <Route exact path='/' render={() => (<Redirect to='/events' />)} />
              <Route path='/event' component={EventDetail} />
              {AuthHelper.getRole() !== 'Guest' && <Route path='/newEvent' component={NewEvent} />}
              {AuthHelper.getRole() !== 'Guest' &&<Route path='/editEvent' component={EditEvent} />}

              {/*AuthHelper.getRole() === 'Admin' || AuthHelper.getRole() === 'Account manager' && <Route path='/users' component={Users} />*/}
              <Route path='/user' component={Profile} />
              <Route path='/signUp' component={SignUp} />
              <Route path='/signIn' component={SignIn} />
              {AuthHelper.getRole() !== 'Guest' && <Route path='/editProfile' component={EditProfile} />}
              {AuthHelper.getRole() !== 'Guest' && <Route path='/changePassword' component={ChangePassword} />}
              {/*{AuthHelper.getRole() === 'Account manager' && <Route path='/roles' component={Roles} />}
              {AuthHelper.getRole() === 'Admin' || AuthHelper.getRole() === 'Account manager' && <Route path='/blocking' component={Blocking} />}*/}

              {AuthHelper.getRole() === 'Admin' && <Route path='/categories' component={Categories} />}
              {AuthHelper.getRole() === 'Admin' && <Route path='/category' component={CategoryDetail} />}
              {AuthHelper.getRole() === 'Admin' && <Route path='/newCategory' component={NewCategory} />}
              {AuthHelper.getRole() === 'Admin' && <Route path='/editCategory' component={EditCategory} />}
              <Route component={Error404} />
            </Switch>
        </Layout>
    );
  }
}
