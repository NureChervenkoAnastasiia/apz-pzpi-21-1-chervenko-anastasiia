/*import React from 'react';
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

export default App;*/


document.addEventListener("DOMContentLoaded", function() {
  var ordersPage = document.getElementById("orders");
  var bookingsPage = document.getElementById("bookings");
  var tablesPage = document.getElementById("tables");
  var staffPage = document.getElementById("staff");
  var schedulePage = document.getElementById("schedule");
  var productsPage = document.getElementById("products");
  var restaurantPage = document.getElementById("restaurant");
  var databasePage = document.getElementById("database");
  var logoutPage = document.getElementById("logout");

  const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        const userData = JSON.parse(localStorage.getItem('userData'));
        if (!userData || !userData.nameid) {
            console.error('Error: User data not found in localStorage');
            return null;
        }
        return userData;
    };

    const fetchStaffPosition = async () => {
        const userData = getUserData();
        if (!userData) return null;

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        try {
            const response = await fetch(`https://localhost:7206/api/staff/${userData.nameid}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staffData = await response.json();
                return staffData.position;
            } else {
                const error = await response.text();
                console.error('Error:', error);
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    /*const positionUrl = "./pages/";
    if (fetchStaffPosition() === "admin"){
        positionUrl = positionUrl + "admin/";
    }else if (fetchStaffPosition === "worker"){
             positionUrl = positionUrl + "staff/";
    }else{
      console.error('Error: user position not founded')
    }*/

  ordersPage.addEventListener("click", function() {
    window.location.href = "orders.html";
  });

  bookingsPage.addEventListener("click", function() {
    window.location.href = "bookings.html";
  });

  tablesPage.addEventListener("click", function() {
    window.location.href = "tables.html";
  });

  staffPage.addEventListener("click", function() {
    window.location.href = "staff.html";
  });

  schedulePage.addEventListener("click", function() {
    window.location.href = "schedule.html";
  });

  productsPage.addEventListener("click", function() {
    window.location.href = "ProductsPage.html";
  });

  restaurantPage.addEventListener("click", function() {
    window.location.href = "restaurant.html";
  });

  restaurantPage.addEventListener("click", function() {
    window.location.href = "MenuPage.html";
  });

  databasePage.addEventListener("click", function() {
    window.location.href = "database.html";
  });

  logoutPage.addEventListener("click", function() {
    window.location.href = "LoginPage.html";
  });
});


/*
import AdminMenuPage from './pages/AdminMenuPage.html';
import AdminOrdersPage from './pages/AdminOrdersPage.html';
import StaffLoginPage from './pages/StaffLoginPage.html';
import StaffProfilePage from './pages/StaffProfilePage.html';
import AdminProductsPage from './pages/admin/Productpage.html'

function App() {
    const root = document.getElementById('root');

    function renderPage(page) {
        root.innerHTML = '';
        root.appendChild(page.cloneNode(true));
    }

    function navigate(path) {
        window.history.pushState({}, '', path);
        const route = window.location.pathname;
        switch (route) {
            case '/admin-menu':
                renderPage(AdminMenuPage);
                break;
            case '/admin-orders':
                renderPage(AdminOrdersPage);
                break;
            case '/staff-login':
                renderPage(StaffLoginPage);
                break;
            case '/staff-profile':
                renderPage(StaffProfilePage);
                break;
            default:
                renderPage(StaffLoginPage);
                break;
        }
    }

    window.navigateTo = navigate;

    window.addEventListener('popstate', () => navigate(window.location.pathname));
    navigate(window.location.pathname);
}

export default App;*/