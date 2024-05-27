document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Product/';
    const sortButton = document.getElementById('sort-button');
    const productsTableBody = document.querySelector('#products-table tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

    const fetchProducts = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                let products = await response.json();

                if (!Array.isArray(products)) {
                    console.error('Fetched products is not an array:', products);
                    products = [];
                }

                displayProducts(products);
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const displayProducts = (products) => {
        productsTableBody.innerHTML = '';

        products.forEach(product => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${product.name}</td>
                <td>${product.amount}</td>
                <td><button class="btn-edit" data-productid="${product.id}">Edit</button></td>
                <td><button class="btn-delete" data-productid="${product.id}">Delete</button></td>
            `;
            productsTableBody.appendChild(row);
        });
    };

    const handleDelete = async (productId) => {
        if (!productId) {
            console.error('Error: Product ID is undefined');
            return;
        }

        try {
            const token = getToken();
            const response = await fetch(`${apiUrl}${productId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Product was deleted successfully!');
                await fetchProducts(); // Refetch products instead of reloading the page
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (productId) => {
        if (!productId) {
            console.error('Error: Product ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-productid="${productId}"]`).parentNode.parentNode;
        const [nameCell, amountCell, editButtonCell, deleteButtonCell] = row.cells;

        const name = nameCell.textContent;
        const amount = amountCell.textContent;

        nameCell.innerHTML = `<input type="text" value="${name}">`;
        amountCell.innerHTML = `<input type="number" value="${amount}">`;

        editButtonCell.innerHTML = `<button class="btn-save" data-productid="${productId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-productid="${productId}">Cancel</button>`;
    };

    const handleSave = async (productId) => {
        if (!productId) {
            console.error('Error: Product ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-productid="${productId}"]`).parentNode.parentNode;
        const token = getToken();
        const name = row.cells[0].querySelector('input').value;
        const amount = row.cells[1].querySelector('input').value;

        if (!name || !amount) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${productId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    name: name,
                    amount: amount,
                })
            });

            if (response.ok) {
                alert('Product was updated successfully!');
                await fetchProducts(); // Refetch products instead of reloading the page
            } else {
                const error = await response.json();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (productId) => {
        if (!productId) {
            console.error('Error: Product ID is undefined');
            return;
        }

        const row = document.querySelector(`button[data-productid="${productId}"]`).parentNode.parentNode;
        const name = row.cells[0].querySelector('input').value;
        const amount = row.cells[1].querySelector('input').value;

        row.innerHTML = `
            <td>${name}</td>
            <td>${amount}</td>
            <td><button class="btn-edit" data-productid="${productId}">Edit</button></td>
            <td><button class="btn-delete" data-productid="${productId}">Delete</button></td>
        `;
    };

    const handleAdd = async () => {
        try {
            const token = getToken();
            const name = document.getElementById('input-name').value;
            const amount = document.getElementById('input-amount').value;

            if (!name || !amount) {
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
                    name: name,
                    amount: amount,
                })
            });

            if (response.ok) {
                alert('Product was added successfully!');
                await fetchProducts(); // Refetch products instead of reloading the page
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    // Event delegation for dynamic buttons
    productsTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const productId = target.getAttribute('data-productid');
        
        if (target.classList.contains('btn-delete')) {
            handleDelete(productId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(productId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(productId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(productId);
        }
    });

    // Add button event listener
    addButton.addEventListener('click', handleAdd);

    const sortProducts = (products, sortParam) => {
        switch (sortParam) {
            case 'name-asc':
                return products.sort((a, b) => a.name.localeCompare(b.name));
            case 'name-desc':
                return products.sort((a, b) => b.name.localeCompare(a.name));
            case 'amount-asc':
                return products.sort((a, b) => a.amount - b.amount);
            case 'amount-desc':
                return products.sort((a, b) => b.amount - a.amount);
            default:
                return products;
        }
    };

    const handleSort = () => {
        const sortOptions = document.querySelectorAll('input[name="sort"]');
        let selectedSort = '';
        sortOptions.forEach(option => {
            if (option.checked) {
                selectedSort = option.value;
            }
        });
        const sortedProducts = sortProducts(products, selectedSort);
        displayProducts(sortedProducts);
    };

    sortButton.addEventListener('click', handleSort);

    // Fetch products initially
    await fetchProducts();
});