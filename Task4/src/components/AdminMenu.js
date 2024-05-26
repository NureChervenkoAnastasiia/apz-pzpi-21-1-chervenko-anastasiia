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

    function handleEdit(menuId) {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;

        const nameCell = row.cells[0];
        const sizeCell = row.cells[1];
        const priceCell = row.cells[2];
        const infoCell = row.cells[3];
        const typeCell = row.cells[4];
        const editButtonCell = row.cells[5];
        const deleteButtonCell = row.cells[6];

        const name = nameCell.textContent;
        const size = sizeCell.textContent;
        const price = priceCell.textContent;
        const info = infoCell.textContent;
        const type = typeCell.textContent;

        nameCell.innerHTML = `<input type="text" value="${name}">`;
        sizeCell.innerHTML = `<input type="text" value="${size}">`;
        priceCell.innerHTML = `<input type="text" value="${price}">`;
        infoCell.innerHTML = `<input type="text" value="${info}">`;
        typeCell.innerHTML = `
            <select>
                <option value="Перші страви" ${type === 'Перші страви' ? 'selected' : ''}>Перші страви</option>
                <option value="Другі страви" ${type === 'Другі страви' ? 'selected' : ''}>Другі страви</option>
                <option value="Напої" ${type === 'Напої' ? 'selected' : ''}>Напої</option>
            </select>
        `;

        editButtonCell.innerHTML = `<button class="btn-save" data-menuid="${menuId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-menuid="${menuId}">Cancel</button>`;
    }

    async function handleSave(menuId) {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }
    
        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;
    
        const token = localStorage.getItem('token');
        const name = row.cells[0].querySelector('input').value;
        const size = row.cells[1].querySelector('input').value;
        const price = row.cells[2].querySelector('input').value;
        const info = row.cells[3].querySelector('input').value;
        const type = row.cells[4].querySelector('select').value;
    
        if (!name || !size || !price || !info || !type) {
            alert('Please fill in all fields');
            return;
        }
    
        console.log("fields:", name, size, price, info, type);
    
        try {
            const restaurantId = await fetchStaff(); // Очікування результату виклику
            if (!restaurantId) {
                console.error('Error: Could not fetch restaurant ID');
                return;
            }
            console.log(restaurantId);
    
            const response = await fetch(`${apiUrl}${menuId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    restaurantId: restaurantId,
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
                const error = await response.json(); // Parsing response body as JSON
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    }

    function handleCancel(menuId) {
        if (!menuId) {
            console.error('Error: Menu ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-menuid="${menuId}"]`).parentNode.parentNode;

        const name = row.cells[0].querySelector('input').value;
        const size = row.cells[1].querySelector('input').value;
        const price = row.cells[2].querySelector('input').value;
        const info = row.cells[3].querySelector('input').value;
        const type = row.cells[4].querySelector('select').value;

        row.innerHTML = `
            <td>${name}</td>
            <td>${size}</td>
            <td>${price}</td>
            <td>${info}</td>
            <td>${type}</td>
            <td><button class="btn-edit" data-menuid="${menuId}">Edit</button></td>
            <td><button class="btn-delete" data-menuid="${menuId}">Delete</button></td>
        `;
    }

    filterButton.addEventListener('click', async function() {
        const selectedFilter = getSelectedFilter();
        const restaurantId = await fetchStaff();
        if (restaurantId) {
            const endpoint = getEndpoint(selectedFilter, restaurantId);
            fetchMenu(endpoint);
        }
    });


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

            const response = await fetch(`https://localhost:7206/api/Menu`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    restaurantId: restaurantId,
                    name: name,
                    size: size,
                    price: price,
                    info: info,
                    type: type,
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
        }else if (target.classList.contains('btn-save')) {
            const menuId = target.dataset.menuid;
            handleSave(menuId);
        }else if (target.classList.contains('btn-cancel')) {
            const menuId = target.dataset.menuid;
            handleCancel(menuId);
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

