document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Booking/';
    const tablesApiUrl = 'https://localhost:7206/api/Table/';
    const guestsApiUrl = 'https://localhost:7206/api/Guest/';
    const bookingsTableBody = document.querySelector('#bookings-table tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

    const fetchBookings = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const bookings = await response.json();
                displayBookings(bookings);
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

    const fetchGuests = async () => {
        try {
            const response = await fetch(guestsApiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const guests = await response.json();
                populateGuestsDropdown(guests);
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

    const populateGuestsDropdown = (guests) => {
        const guestDropdown = document.getElementById('input-guest');
        guestDropdown.innerHTML = '';

        guests.forEach(guest => {
            const option = document.createElement('option');
            option.value = guest.id;
            option.textContent = guest.name;
            guestDropdown.appendChild(option);
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

    const getGuestNameById = async (guestId) => {
        try {
            const response = await fetch(`${guestsApiUrl}${guestId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const guest = await response.json();
                return guest.name;
            } else {
                console.error('Error:', await response.text());
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const displayBookings = async (bookings) => {
        bookingsTableBody.innerHTML = '';
        for (const booking of bookings) {
            const tableNumber = await getTableNumberById(booking.tableId);
            const guestName = await getGuestNameById(booking.guestId);
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
        try {
            const response = await fetch(`${apiUrl}${bookingId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Booking was deleted successfully!');
                await fetchBookings(); 
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
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

        // Set the correct table and guest selection
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

        try {
            const response = await fetch(`${apiUrl}${bookingId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ tableId, guestId, bookingDateTime, personsCount, comment })
            });

            if (response.ok) {
                alert('Booking was updated successfully!');
                await fetchBookings(); // Refetch bookings instead of reloading the page
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (bookingId) => {
        fetchBookings(); // Simply refetch the bookings to reset the table
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

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ tableId, guestId, bookingDateTime, personsCount, comment })
            });

            if (response.ok) {
                alert('Booking was added successfully!');
                await fetchBookings(); 
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
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
            handleCancel(bookingId);
        }
    });

    addButton.addEventListener('click', handleAdd);

    await fetchTables();
    await fetchGuests();
    await fetchBookings();
});
