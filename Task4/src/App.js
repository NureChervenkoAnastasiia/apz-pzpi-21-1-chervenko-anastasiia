document.addEventListener("DOMContentLoaded", function() {
  var ordersPage = document.getElementById("orders");
  var bookingsPage = document.getElementById("bookings");
  var tablesPage = document.getElementById("tables");
  var staffPage = document.getElementById("staff");
  var schedulePage = document.getElementById("schedule");
  var productsPage = document.getElementById("products");
  var restaurantPage = document.getElementById("restaurant");
  var menuPage = document.getElementById("menu");
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

  ordersPage.addEventListener("click", function() {
    window.location.href = "OrdersPage.html";
  });

  bookingsPage.addEventListener("click", function() {
    window.location.href = "BookingsPage.html";
  });

  tablesPage.addEventListener("click", function() {
    window.location.href = "TablesPage.html";
  });

  staffPage.addEventListener("click", function() {
    window.location.href = "StaffPage.html";
  });

  schedulePage.addEventListener("click", function() {
    window.location.href = "SchedulePage.html";
  });

  productsPage.addEventListener("click", function() {
    window.location.href = "ProductsPage.html";
  });

  restaurantPage.addEventListener("click", function() {
    window.location.href = "RestaurantPage.html";
  });

  menuPage.addEventListener("click", function() {
    window.location.href = "MenuPage.html";
  });

  databasePage.addEventListener("click", function() {
    window.location.href = "DataManagingPage.html";
  });

  logoutPage.addEventListener("click", function() {
    window.location.href = "LoginPage.html";
  });
});