document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Menu/';
    const filterButton = document.getElementById('filter-button');
    const menuTableBody = document.querySelector('#menu-table tbody');
    const menuTableContainer = document.querySelector('.table-container');
    const popularityContainer = document.getElementById('popularity-container');

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        const userData = JSON.parse(localStorage.getItem('userData'));
        if (!userData || !userData.nameid) {
            console.error('Error: User data not found in localStorage');
            return null;
        }
        return userData;
    };

    const fetchWithAuth = async (url, options = {}) => {
        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
            ...options.headers,
        };

        try {
            const response = await fetch(url, { ...options, headers });
            if (!response.ok) {
                const error = await response.text();
                console.error('Error:', error);
                return null;
            }
            return await response.json();
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const fetchStaff = async () => {
        const userData = getUserData();
        if (!userData) return null;

        const staffData = await fetchWithAuth(`https://localhost:7206/api/staff/${userData.nameid}`);
        console.log('Staff Data:', staffData);
        return staffData ? staffData.restaurantId : null;
    };

    const fetchMenu = async (endpoint) => {
        console.log('Fetching menu from endpoint:', endpoint);
        const data = await fetchWithAuth(endpoint);
        console.log('Menu Data:', data);
        if (data) {
            displayMenu(data);
        } else {
            console.error('Error: Failed to fetch menu data');
        }
    };

    const displayMenu = (menuItems) => {
        menuTableBody.innerHTML = '';
        popularityContainer.style.display = 'none';
        menuTableContainer.style.display = 'block';

        if (menuItems.length === 0) {
            console.log('No menu items to display');
            return;
        }

        menuItems.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.size}</td>
                <td>${item.price}</td>
                <td>${item.info}</td>
                <td>${item.type}</td>
            `;
            menuTableBody.appendChild(row);
        });
    };

    const getSelectedFilter = () => document.querySelector('input[name="filter"]:checked').value;

    const getEndpoint = (filter, restaurantId) => {
        const endpoints = {
            'first-dishes': `${apiUrl}restaurant/${restaurantId}/first-dishes`,
            'second-dishes': `${apiUrl}restaurant/${restaurantId}/second-dishes`,
            'drinks': `${apiUrl}restaurant/${restaurantId}/drinks`,
            'default': `${apiUrl}restaurant/${restaurantId}/menu`,
        };
        return endpoints[filter] || endpoints['default'];
    };

    filterButton.addEventListener('click', async () => {
        const filter = getSelectedFilter();
        const restaurantId = await fetchStaff();
        if (restaurantId) {
            await fetchMenu(getEndpoint(filter, restaurantId));
        } else {
            console.error('Error: Could not fetch restaurant ID');
        }
    });

    const restaurantId = await fetchStaff();
    if (restaurantId) {
        await fetchMenu(getEndpoint('menu', restaurantId));
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }
});