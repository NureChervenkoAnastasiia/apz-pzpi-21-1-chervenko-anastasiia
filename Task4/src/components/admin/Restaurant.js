document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Restaurants/';
    const restaurantContainer = document.querySelector('.restaurant-container');

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        const userData = JSON.parse(localStorage.getItem('userData'));
        if (!userData || !userData.nameid) {
            console.error('Error: User data not found in localStorage');
            return null;
        }
        return userData;
    };

    const fetchRestaurantId = async () => {
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
                return staffData.restaurantId;
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

    const fetchRestaurantInfo = async (restaurantId) => {
        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${restaurantId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const restaurantData = await response.json();
                displayRestaurantInfo(restaurantData);
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const displayRestaurantInfo = (restaurant) => {
        restaurantContainer.innerHTML = `
            <h2>${restaurant.name}</h2>
            <p><strong>Address:</strong> ${restaurant.address}</p>
            <p><strong>Phone:</strong> ${restaurant.phone}</p>
            <p><strong>Email:</strong> <a href="mailto:${restaurant.email}">${restaurant.email}</a></p>
            <p><strong>Description:</strong> ${restaurant.info}</p>
            <p><strong>Cuisine:</strong> ${restaurant.cuisine.join(', ')}</p>
        `;
    };

    const restaurantId = await fetchRestaurantId();
    if (restaurantId) {
        await fetchRestaurantInfo(restaurantId);
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }
});
