/*import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import StaffHomePage from './pages/StaffHomePage';
import StaffLoginPage from './pages/StaffLoginPage';
import AdminHomePage from './pages/AdminHomePage';

function App() {
  return (
    <Router>
      <Switch>
        <Route path="/staff-login" exact component={StaffLoginPage} />
        <Route path="/staff-home" component={StaffHomePage} />
        <Route path="/admin-home" component={AdminHomePage} />
      </Switch>
    </Router>
  );
}

export default App;*/

import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Login from './components/Login';
import StaffHome from './components/StaffHome';
import AdminHome from './components/AdminHome';
import './App.css';

function App() {
    return (
        <Router>
            <Switch>
                <Route path="/" exact component={Login} />
                <Route path="/staff-home" component={StaffHome} />
                <Route path="/admin-home" component={AdminHome} />
            </Switch>
        </Router>
    );
}

export default App;

