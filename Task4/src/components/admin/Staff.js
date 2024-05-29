document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Staff/';
    const restaurantsApiUrl = 'https://localhost:7206/api/Restaurants/';
    const workingHoursApiUrl = 'https://localhost:7206/api/Staff/weekly-working-hours';
    const staffTableBody = document.querySelector('#staff-table tbody');
    const addButton = document.querySelector('.btn-add');
    const fetchHoursButton = document.querySelector('.btn-fetch-hours');
    const inputDate = document.getElementById('input-date');
    const workingHoursContainer = document.getElementById('working-hours-container');

    const getToken = () => localStorage.getItem('token');

    const fetchStaff = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staff = await response.json();
                displayStaff(staff);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const fetchRestaurants = async () => {
        try {
            const response = await fetch(restaurantsApiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const restaurants = await response.json();
                populateRestaurantsDropdown(restaurants);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const populateRestaurantsDropdown = (restaurants) => {
        const restaurantDropdown = document.getElementById('input-restaurant');
        restaurantDropdown.innerHTML = '';

        restaurants.forEach(restaurant => {
            const option = document.createElement('option');
            option.value = restaurant.id;
            option.textContent = restaurant.name;
            restaurantDropdown.appendChild(option);
        });
    };

    const getRestaurantNameById = async (restaurantId) => {
        try {
            const response = await fetch(`${restaurantsApiUrl}${restaurantId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const restaurant = await response.json();
                return restaurant.name;
            } else {
                console.error('Error:', await response.text());
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const displayStaff = async (staff) => {
        staffTableBody.innerHTML = '';
        for (const member of staff) {
            const restaurantName = await getRestaurantNameById(member.restaurantId);
            if (!restaurantName) {
                console.error(`Restaurant not found for staff member with ID ${member.id}`);
                continue;
            }

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${member.name}</td>
                <td>${member.position}</td>
                <td>${member.hourlySalary}</td>
                <td>${member.phone}</td>
                <td>${member.attendanceCard}</td>
                <td>${member.login}</td>
                <td>${restaurantName}</td>
                <td><button class="btn-edit" data-staffid="${member.id}">Edit</button></td>
                <td><button class="btn-delete" data-staffid="${member.id}">Delete</button></td>
            `;
            staffTableBody.appendChild(row);
        }
    };

    const handleDelete = async (staffId) => {
        try {
            const response = await fetch(`${apiUrl}${staffId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Staff member was deleted successfully!');
                await fetchStaff();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (staffId) => {
        const row = document.querySelector(`button[data-staffid="${staffId}"]`).parentNode.parentNode;
        const [nameCell, positionCell, salaryCell, phoneCell, cardCell, loginCell, restaurantCell, editButtonCell, deleteButtonCell] = row.cells;

        const name = nameCell.textContent;
        const position = positionCell.textContent;
        const salary = salaryCell.textContent;
        const phone = phoneCell.textContent;
        const attendanceCard = cardCell.textContent;
        const login = loginCell.textContent;
        const restaurantName = restaurantCell.textContent;

        nameCell.innerHTML = `<input type="text" value="${name}">`;
        positionCell.innerHTML = `<select>${document.getElementById('input-position').innerHTML}</select>`;
        positionCell.querySelector('select').value = position;
        salaryCell.innerHTML = `<input type="number" value="${salary}">`;
        phoneCell.innerHTML = `<input type="number" value="${phone}">`;
        cardCell.innerHTML = `<input type="number" value="${attendanceCard}">`;
        loginCell.innerHTML = `<input type="text" value="${login}">`;
        restaurantCell.innerHTML = `<select>${document.getElementById('input-restaurant').innerHTML}</select>`;
        restaurantCell.querySelector('select').value = restaurantName;

        editButtonCell.innerHTML = `<button class="btn-save" data-staffid="${staffId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-staffid="${staffId}">Cancel</button>`;
    };

    const handleSave = async (staffId) => {
        const row = document.querySelector(`button[data-staffid="${staffId}"]`).parentNode.parentNode;
        const name = row.cells[0].querySelector('input').value;
        const position = row.cells[1].querySelector('select').value;
        const hourlySalary = row.cells[2].querySelector('input').value;
        const phone = row.cells[3].querySelector('input').value;
        const attendanceCard = row.cells[4].querySelector('input').value;
        const login = row.cells[5].querySelector('input').value;
        const restaurantId = row.cells[6].querySelector('select').value;

        if (!name || !position || !hourlySalary || !phone || !attendanceCard || !login || !restaurantId) {
            alert('Please fill in all fields');
            return;
        }

        let password = null;
        if (confirm('Do you want to change the password?')) {
            password = prompt('Please enter the new password:');
        }

        const payload = { name, position, hourlySalary, phone, attendanceCard, login, restaurantId };
        if (password) {
            payload.password = password;
        }

        try {
            const response = await fetch(`${apiUrl}${staffId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                alert('Staff member was updated successfully!');
                await fetchStaff(); // Refetch staff instead of reloading the page
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (staffId) => {
        fetchStaff(); // Simply refetch the staff to reset the table
    };

    const handleAdd = async () => {
        const name = document.getElementById('input-name').value;
        const position = document.getElementById('input-position').value;
        const hourlySalary = document.getElementById('input-salary').value;
        const phone = document.getElementById('input-phone').value;
        const attendanceCard = document.getElementById('input-card').value;
        const login = document.getElementById('input-login').value;
        const password = prompt('Please enter a password for the new staff member:');
        const restaurantId = document.getElementById('input-restaurant').value;

        if (!name || !position || !hourlySalary || !phone || !attendanceCard || !login || !password || !restaurantId) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}register`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ name, position, hourlySalary, phone, attendanceCard, login, password, restaurantId })
            });

            if (response.ok) {
                alert('Staff member was added successfully!');
                document.getElementById('input-name').value = '';
                document.getElementById('input-position').value = '';
                document.getElementById('input-salary').value = '';
                document.getElementById('input-phone').value = '';
                document.getElementById('input-card').value = '';
                document.getElementById('input-login').value = '';
                document.getElementById('input-restaurant').value = '';
                await fetchStaff();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleFetchHours = async () => {
        const selectedDate = inputDate.value;
        if (!selectedDate) {
            alert('Please select a date');
            return;
        }
    
        const url = `${workingHoursApiUrl}?date=${encodeURIComponent(selectedDate)}`;
        console.log(`Fetching working hours from URL: ${url}`);
        try {
            const response = await fetch(url, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });
    
            if (response.ok) {
                const workingHours = await response.json();
                console.log('Received working hours:', workingHours);
                displayWorkingHours(workingHours);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };
    
    const displayWorkingHours = (workingHours) => {
        workingHoursContainer.innerHTML = '';
        console.log('Displaying working hours:', workingHours);
        workingHoursContainer.style.display = 'block';
    
        workingHours.forEach(entry => {
            const p = document.createElement('p');
            p.innerHTML = `${entry.name} - ${entry.totalWorkingHours} годин`;
            workingHoursContainer.appendChild(p);
        });
    };

    staffTableBody.addEventListener('click', async (event) => {
        const { target } = event;
        const staffId = target.getAttribute('data-staffid');

        if (target.classList.contains('btn-delete')) {
            if (confirm('Are you sure you want to delete this staff member?')) {
                await handleDelete(staffId);
            }
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(staffId);
        } else if (target.classList.contains('btn-save')) {
            await handleSave(staffId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(staffId);
        }
    });

    addButton.addEventListener('click', handleAdd);
    fetchHoursButton.addEventListener('click', handleFetchHours);

    fetchStaff();
    fetchRestaurants();
});