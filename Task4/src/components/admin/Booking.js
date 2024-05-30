document.addEventListener('DOMContentLoaded', async () => {
    const apiUrl = 'https://localhost:7206/api/Booking/';
    const tablesApiUrl = 'https://localhost:7206/api/Table/';
    const guestsApiUrl = 'https://localhost:7206/api/Guest/';
    const bookingsTableBody = document.querySelector('#bookings-table tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

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

    const fetchBookings = async () => {
        const bookings = await fetchWithAuth(apiUrl);
        if (bookings) {
            displayBookings(bookings);
        }
    };

    const fetchTables = async () => {
        const tables = await fetchWithAuth(tablesApiUrl);
        if (tables) {
            populateDropdown('input-table', tables, 'number');
        }
    };

    const fetchGuests = async () => {
        const guests = await fetchWithAuth(guestsApiUrl);
        if (guests) {
            populateDropdown('input-guest', guests, 'name');
        }
    };

    const populateDropdown = (elementId, items, textProperty) => {
        const dropdown = document.getElementById(elementId);
        dropdown.innerHTML = '';

        items.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id;
            option.textContent = item[textProperty];
            dropdown.appendChild(option);
        });
    };

    const getItemPropertyById = async (url, id, property) => {
        const item = await fetchWithAuth(`${url}${id}`);
        return item ? item[property] : null;
    };

    const displayBookings = async (bookings) => {
        bookingsTableBody.innerHTML = '';

        for (const booking of bookings) {
            const tableNumber = await getItemPropertyById(tablesApiUrl, booking.tableId, 'number');
            const guestName = await getItemPropertyById(guestsApiUrl, booking.guestId, 'name');
            if (!tableNumber || !guestName) {
                console.error(`Table or guest not found for booking with ID ${booking.id}`);
                continue;
            }

            const formattedDateTime = formatDateTime(booking.bookingDateTime);

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${tableNumber}</td>
                <td>${guestName}</td>
                <td>${formattedDateTime}</td>
                <td>${booking.personsCount}</td>
                <td>${booking.comment}</td>
                <td><button class="btn-edit" data-bookingid="${booking.id}">Edit</button></td>
                <td><button class="btn-delete" data-bookingid="${booking.id}">Delete</button></td>
            `;
            bookingsTableBody.appendChild(row);
        }
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA'); // YYYY-MM-DD format
        const formattedTime = date.toLocaleTimeString('en-GB'); // HH:MM:SS format
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleDelete = async (bookingId) => {
        const response = await fetchWithAuth(`${apiUrl}${bookingId}`, { method: 'DELETE' });
        if (response) {
            alert('Booking was deleted successfully!');
            await fetchBookings();
        }
    };

    const handleEdit = (bookingId) => {
        const row = document.querySelector(`button[data-bookingid="${bookingId}"]`).parentNode.parentNode;
        const [tableCell, guestCell, dateTimeCell, personsCell, commentCell, editButtonCell, deleteButtonCell] = row.cells;

        const tableNumber = tableCell.textContent;
        const guestName = guestCell.textContent;
        const dateTime = dateTimeCell.textContent;
        const personsCount = personsCell.textContent;
        const comment = commentCell.textContent;

        tableCell.innerHTML = `<select>${document.getElementById('input-table').innerHTML}</select>`;
        guestCell.innerHTML = `<select>${document.getElementById('input-guest').innerHTML}</select>`;
        dateTimeCell.innerHTML = `<input type="datetime-local" value="${formatDateTime(dateTime)}">`;
        personsCell.innerHTML = `<input type="number" value="${personsCount}">`;
        commentCell.innerHTML = `<input type="text" value="${comment}">`;

        tableCell.querySelector('select').value = tableNumber;
        guestCell.querySelector('select').value = guestName;

        editButtonCell.innerHTML = `<button class="btn-save" data-bookingid="${bookingId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-bookingid="${bookingId}">Cancel</button>`;
    };

    const handleSave = async (bookingId) => {
        const row = document.querySelector(`button[data-bookingid="${bookingId}"]`).parentNode.parentNode;
        const tableId = row.cells[0].querySelector('select').value;
        const guestId = row.cells[1].querySelector('select').value;
        const bookingDateTime = row.cells[2].querySelector('input').value;
        const personsCount = row.cells[3].querySelector('input').value;
        const comment = row.cells[4].querySelector('input').value;

        if (!tableId || !guestId || !bookingDateTime || !personsCount) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchWithAuth(`${apiUrl}${bookingId}`, {
            method: 'PUT',
            body: JSON.stringify({ tableId, guestId, bookingDateTime, personsCount, comment })
        });

        if (response) {
            alert('Booking was updated successfully!');
            await fetchBookings();
        }
    };

    const handleCancel = () => {
        fetchBookings();
    };

    const handleAdd = async () => {
        const tableId = document.getElementById('input-table').value;
        const guestId = document.getElementById('input-guest').value;
        const bookingDateTime = document.getElementById('input-datetime').value;
        const personsCount = document.getElementById('input-number').value;
        const comment = document.getElementById('input-text').value;

        if (!tableId || !guestId || !bookingDateTime || !personsCount) {
            alert('Please fill in all fields');
            return;
        }

        const response = await fetchWithAuth(apiUrl, {
            method: 'POST',
            body: JSON.stringify({ tableId, guestId, bookingDateTime, personsCount, comment })
        });

        if (response) {
            alert('Booking was added successfully!');
            await fetchBookings();
        }
    };

    bookingsTableBody.addEventListener('click', async (event) => {
        const bookingId = event.target.getAttribute('data-bookingid');
        if (event.target.classList.contains('btn-edit')) {
            handleEdit(bookingId);
        } else if (event.target.classList.contains('btn-delete')) {
            await handleDelete(bookingId);
        } else if (event.target.classList.contains('btn-save')) {
            await handleSave(bookingId);
        } else if (event.target.classList.contains('btn-cancel')) {
            handleCancel();
        }
    });

    addButton.addEventListener('click', handleAdd);

    await fetchTables();
    await fetchGuests();
    await fetchBookings();
});