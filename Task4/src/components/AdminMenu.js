document.addEventListener('DOMContentLoaded', async function() {
    const filterButton = document.getElementById('filter-button');
    const menuTableBody = document.querySelector('#menu-table tbody');
    const menuTableContainer = document.querySelector('.table-container');
    const popularityContainer = document.getElementById('popularity-container');
    const apiUrl = 'https://localhost:7206/api/Menu/';

    async function fetchStaff() {
        try {
            const userData = JSON.parse(localStorage.getItem('userData'));
            if (!userData || !userData.nameid) {
                console.error('Error: User data not found in localStorage');
                return;
            }

            const userId = userData.nameid;
            const token = localStorage.getItem('token');
            if (!token) {
                console.error('Error: Token not found in localStorage');
                return;
            }

            const response = await fetch(`https://localhost:7206/api/staff/${userId}`, {
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
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    }

    async function fetchMenu(endpoint) {
        try {
            const response = await fetch(endpoint, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.ok) {
                const data = await response.json();
                if (endpoint.includes('dishes-rating')) {
                    displayPopularity(data);
                } else {
                    displayMenu(data);
                }
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    }

    function displayMenu(menuItems) {
        menuTableBody.innerHTML = '';
        popularityContainer.style.display = 'none';
        menuTableContainer.style.display = 'block';

        menuItems.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.name}</td>
                <td>${item.size}</td>
                <td>${item.price}</td>
                <td>${item.info}</td>
                <td>${item.type}</td>
                <td><button class="btn-edit" data-menuid="${item.id}">Edit</button></td>
                <td><button class="btn-delete" data-menuid="${item.id}">Delete</button></td>
            `;
            menuTableBody.appendChild(row);
        });
    }

    function displayPopularity(popularityItems) {
        menuTableContainer.style.display = 'none';
        popularityContainer.style.display = 'block';
        popularityContainer.innerHTML = '';

        popularityItems.forEach(item => {
            const popularityEntry = document.createElement('div');
            popularityEntry.textContent = `${item.name} - ${item.ordersCount} замовлень`;
            popularityContainer.appendChild(popularityEntry);
        });
    }

    function getSelectedFilter() {
        const selectedFilter = document.querySelector('input[name="filter"]:checked').value;
        return selectedFilter;
    }

    function getEndpoint(filter, restaurantId) {
        switch (filter) {
            case 'first-dishes':
                return `${apiUrl}restaurant/${restaurantId}/first-dishes`;
            case 'second-dishes':
                return `${apiUrl}restaurant/${restaurantId}/second-dishes`;
            case 'drinks':
                return `${apiUrl}restaurant/${restaurantId}/drinks`;
            case 'popularity':
                return `${apiUrl}restaurant/${restaurantId}/dishes-rating`;
            default:
                return `${apiUrl}restaurant/${restaurantId}/menu`;
        }
    }

    async function fetchMenuItem(menuId) {
        const token = localStorage.getItem('token');
        const response = await fetch(`${apiUrl}${menuId}`, {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (response.ok) {
            return await response.json();
        } else {
            console.error('Error fetching menu item:', await response.text());
            return null;
        }
    }

    function enableEditing(menuItem) {
        const menuId = menuItem.id;

        const nameCell = document.getElementById(`name-${menuId}`);
        const sizeCell = document.getElementById(`size-${menuId}`);
        const priceCell = document.getElementById(`price-${menuId}`);
        const infoCell = document.getElementById(`info-${menuId}`);
        const typeCell = document.getElementById(`type-${menuId}`);

        nameCell.innerHTML = `<input type="text" id="edit-name-${menuId}" value="${menuItem.name}">`;
        sizeCell.innerHTML = `<input type="text" id="edit-size-${menuId}" value="${menuItem.size}">`;
        priceCell.innerHTML = `<input type="text" id="edit-price-${menuId}" value="${menuItem.price}">`;
        infoCell.innerHTML = `<input type="text" id="edit-info-${menuId}" value="${menuItem.info}">`;
        typeCell.innerHTML = `
            <select id="edit-type-${menuId}">
                <option value="Перші страви" ${menuItem.type === 'Перші страви' ? 'selected' : ''}>Перші страви</option>
                <option value="Другі страви" ${menuItem.type === 'Другі страви' ? 'selected' : ''}>Другі страви</option>
                <option value="Напої" ${menuItem.type === 'Напої' ? 'selected' : ''}>Напої</option>
            </select>
        `;

        const editButton = document.querySelector(`button.btn-edit[data-menuid="${menuId}"]`);
        editButton.textContent = 'Save';
        editButton.onclick = () => handleSave(menuId);
    }

    async function handleEdit(menuId) {
        const menuItem = await fetchMenuItem(menuId);
        if (menuItem) {
            enableEditing(menuItem);
        }
    }

    async function handleSave(menuId) {
        const token = localStorage.getItem('token');
        const name = document.getElementById(`edit-name-${menuId}`).value;
        const size = document.getElementById(`edit-size-${menuId}`).value;
        const price = document.getElementById(`edit-price-${menuId}`).value;
        const info = document.getElementById(`edit-info-${menuId}`).value;
        const type = document.getElementById(`edit-type-${menuId}`).value;

        if (!name || !size || !price || !info || !type) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetch(`${apiUrl}/${menuId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                name: name,
                size: size,
                price: price,
                info: info,
                type: type
            })
        });

        if (response.ok) {
            alert('Dish or drink was updated successfully!');
            location.reload();
        } else {
            const error = await response.text();
            console.error('Error:', error);
        }
    }

    async function handleDelete(menuId) {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${apiUrl}${menuId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            if (response.ok) {
                alert('Dish or drink was deleted successfully!');
                location.reload();
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    }

    async function handleAdd() {
        try {
            const token = localStorage.getItem('token');
            const name = document.getElementById('input-name').value;
            const size = document.getElementById('input-size').value;
            const price = document.getElementById('input-price').value;
            const info = document.getElementById('input-info').value;
            const type = document.getElementById('input-type').value;

            if (!name || !size || !price || !info || !type) {
                alert('Please fill in all fields');
                return;
            }

            const restaurantId = await fetchStaff();
            if (!restaurantId) {
                console.error('Error: Could not fetch restaurant ID');
                return;
            }

            const response = await fetch(`${apiUrl}restaurant/${restaurantId}/menu`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    name: name,
                    size: size,
                    price: price,
                    info: info,
                    type: type,
                    RestaurantId: restaurantId // Добавляем RestaurantId в запрос
                })
            });

            if (response.ok) {
                alert('Dish or drink was added successfully!');
                location.reload();
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    }

    filterButton.addEventListener('click', async function() {
        const filter = getSelectedFilter();
        const restaurantId = await fetchStaff();
        if (restaurantId) {
            fetchMenu(getEndpoint(filter, restaurantId));
        } else {
            console.error('Error: Could not fetch restaurant ID');
        }
    });

    menuTableBody.addEventListener('click', function(event) {
        const target = event.target;
        if (target.classList.contains('btn-edit')) {
            const menuId = target.dataset.menuid;
            handleEdit(menuId);
        } else if (target.classList.contains('btn-delete')) {
            const menuId = target.dataset.menuid;
            handleDelete(menuId);
        }
    });

    const addButton = document.querySelector('.btn-add');
    addButton.addEventListener('click', handleAdd);

    const restaurantId = await fetchStaff();
    if (restaurantId) {
        fetchMenu(getEndpoint('menu', restaurantId));
    } else {
        console.error('Error: Could not fetch restaurant ID');
    }
});

