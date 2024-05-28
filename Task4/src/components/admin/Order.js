document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Order/';
    const tablesApiUrl = 'https://localhost:7206/api/Table/';
    const ordersTableBody = document.querySelector('#orders-table tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

    const fetchOrders = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const orders = await response.json();
                displayOrders(orders);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const fetchTables = async () => {
        try {
            const response = await fetch(tablesApiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const tables = await response.json();
                populateTablesDropdown(tables);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const populateTablesDropdown = (tables) => {
        const tableDropdown = document.getElementById('input-table');
        tableDropdown.innerHTML = '';

        tables.forEach(table => {
            const option = document.createElement('option');
            option.value = table.id;
            option.textContent = table.number;
            tableDropdown.appendChild(option);
        });
    };

    const getTableNumberById = async (tableId) => {
        try {
            const response = await fetch(`${tablesApiUrl}${tableId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });
    
            if (response.ok) {
                const table = await response.json();
                return table.number;
            } else {
                console.error('Error:', await response.text());
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const displayOrders = async (orders) => {
        ordersTableBody.innerHTML = '';
        for (const order of orders) {
            const tableNumber = await getTableNumberById(order.tableId);
            if (!tableNumber) {
                console.error(`Table not found for order with ID ${order.id}`);
                continue;
            }

            const formattedDateTime = formatDateTime(order.orderDateTime);

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${order.number}</td>
                <td>${tableNumber}</td>
                <td>${formattedDateTime}</td>
                <td>${order.comment}</td>
                <td>${order.status}</td>
                <td><button class="btn-edit" data-orderid="${order.id}">Edit</button></td>
                <td><button class="btn-delete" data-orderid="${order.id}">Delete</button></td>
            `;
            ordersTableBody.appendChild(row);
        }
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA'); // YYYY-MM-DD format
        const formattedTime = date.toLocaleTimeString('en-GB'); // HH:MM:SS format
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleDelete = async (orderId) => {
        try {
            const response = await fetch(`${apiUrl}${orderId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Order was deleted successfully!');
                await fetchOrders(); 
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).parentNode.parentNode;
        const [numberCell, tableCell, dateTimeCell, commentCell, statusCell, editButtonCell, deleteButtonCell] = row.cells;

        const number = numberCell.textContent;
        const tableNumber = tableCell.textContent;
        const dateTime = dateTimeCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');
        const comment = commentCell.textContent;
        const status = statusCell.textContent;

        numberCell.innerHTML = `<input type="number" value="${number}">`;
        tableCell.innerHTML = `<select>${document.getElementById('input-table').innerHTML}</select>`;
        tableCell.querySelector('select').value = tableNumber;
        dateTimeCell.innerHTML = `<input type="datetime-local" value="${dateTime}">`;
        commentCell.innerHTML = `<input type="text" value="${comment}">`;
        statusCell.innerHTML = `<select>${document.getElementById('input-status').innerHTML}</select>`;
        statusCell.querySelector('select').value = status;

        editButtonCell.innerHTML = `<button class="btn-save" data-orderid="${orderId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-orderid="${orderId}">Cancel</button>`;
    };

    const handleSave = async (orderId) => {
        const row = document.querySelector(`button[data-orderid="${orderId}"]`).parentNode.parentNode;
        const number = row.cells[0].querySelector('input').value;
        const tableId = row.cells[1].querySelector('select').value;
        const orderDateTime = row.cells[2].querySelector('input').value;
        const comment = row.cells[3].querySelector('input').value;
        const status = row.cells[4].querySelector('select').value;

        if (!number || !tableId || !orderDateTime || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${orderId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ number, tableId, orderDateTime, comment, status })
            });

            if (response.ok) {
                alert('Order was updated successfully!');
                await fetchOrders(); // Refetch orders instead of reloading the page
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (orderId) => {
        fetchOrders(); // Simply refetch the orders to reset the table
    };

    const handleAdd = async () => {
        const number = document.getElementById('input-number').value;
        const tableId = document.getElementById('input-table').value;
        const orderDateTime = document.getElementById('input-datetime').value;
        const comment = document.getElementById('input-comment').value;
        const status = document.getElementById('input-status').value;

        if (!number || !tableId || !orderDateTime || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ number, tableId, orderDateTime, comment, status })
            });

            if (response.ok) {
                alert('Order was added successfully!');
                await fetchOrders(); // Refetch orders instead of reloading the page
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    ordersTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const orderId = target.getAttribute('data-orderid');

        if (target.classList.contains('btn-delete')) {
            handleDelete(orderId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(orderId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(orderId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(orderId);
        }
    });

    addButton.addEventListener('click', handleAdd);

    await fetchTables(); // Fetch tables first to populate the dropdown
    await fetchOrders(); // Fetch orders next
});