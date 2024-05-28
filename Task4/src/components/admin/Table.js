document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Table/';
    const sortButton = document.getElementById('sort-button');
    const tablesTableBody = document.querySelector('#tables-table tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

    const fetchTables = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                let tables = await response.json();

                if (!Array.isArray(tables)) {
                    console.error('Fetched tables is not an array:', tables);
                    tables = [];
                }

                displayTables(tables);
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const displayTables = (tables) => {
        tablesTableBody.innerHTML = '';

        tables.forEach(table => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${table.number}</td>
                <td>${table.status}</td>
                <td><button class="btn-edit" data-tableid="${table.id}">Edit</button></td>
                <td><button class="btn-delete" data-tableid="${table.id}">Delete</button></td>
            `;
            tablesTableBody.appendChild(row);
        });
    };

    const handleDelete = async (tableId) => {
        if (!tableId) {
            console.error('Error: Table ID is undefined');
            return;
        }

        try {
            const token = getToken();
            const response = await fetch(`${apiUrl}${tableId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Table was deleted successfully!');
                await fetchTables(); // Refetch tables instead of reloading the page
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (tableId) => {
        if (!tableId) {
            console.error('Error: Table ID is undefined');
            return;
        }
    
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
        const [numberCell, statusCell, editButtonCell, deleteButtonCell] = row.cells;
    
        const number = numberCell.textContent;
        const status = statusCell.textContent;
    
        numberCell.innerHTML = `<input type="text" value="${number}">`;
        statusCell.innerHTML = `<input type="text" value="${status}">`;
    
        editButtonCell.innerHTML = `<button class="btn-save" data-tableid="${tableId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-tableid="${tableId}">Cancel</button>`;
    };

    const handleSave = async (tableId) => {
        if (!tableId) {
            console.error('Error: Table ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
        const token = getToken();
        const number = row.cells[0].querySelector('input').value;
        const status = row.cells[1].querySelector('input').value;

        if (!number || !status) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${tableId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    number: number,
                    status: status,
                })
            });

            if (response.ok) {
                alert('Table was updated successfully!');
                await fetchTables();
            } else {
                const error = await response.json();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (tableId) => {
        if (!tableId) {
            console.error('Error: Table ID is undefined');
            return;
        }
    
        const row = document.querySelector(`button[data-tableid="${tableId}"]`).parentNode.parentNode;
        const number = row.cells[0].querySelector('input').value;
        const status = row.cells[1].querySelector('input').value;
    
        row.innerHTML = `
            <td>${number}</td>
            <td>${status}</td>
            <td><button class="btn-edit" data-tableid="${tableId}">Edit</button></td>
            <td><button class="btn-delete" data-tableid="${tableId}">Delete</button></td>
        `;
    };

    const handleAdd = async () => {
        try {
            const token = getToken();
            const number = document.getElementById('input-number').value;
            const status = document.getElementById('input-status').value;
    
            if (!number || !status) {
                alert('Please fill in all fields');
                return;
            }
    
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    number: number,
                    status: status,
                })
            });
    
            if (response.ok) {
                alert('Table was added successfully!');
                await fetchTables();
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };
    

    // Event delegation for dynamic buttons
    tablesTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const tableId = target.getAttribute('data-tableid');
        
        if (target.classList.contains('btn-delete')) {
            handleDelete(tableId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(tableId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(tableId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(tableId);
        }
    });

    // Add button event listener
    addButton.addEventListener('click', handleAdd);


    // Fetch tables initially
    await fetchTables();
});