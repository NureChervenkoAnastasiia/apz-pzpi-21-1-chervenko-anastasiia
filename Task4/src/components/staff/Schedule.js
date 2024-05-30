document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Schedule/staff/';
    const staffApiUrl = 'https://localhost:7206/api/Staff/';
    const scheduleTableBody = document.querySelector('#schedule tbody');
    const addButton = document.querySelector('.btn-add');
    let currentUser = null;
    let currentStaffName = '';

    const getToken = () => localStorage.getItem('token');

    const decodeToken = (token) => {
        try {
            return jwt_decode(token);
        } catch (error) {
            console.error('Error decoding token:', error);
            return null;
        }
    };

    const fetchSchedules = async (staffId) => {
        try {
            const response = await fetch(`${apiUrl}${staffId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const schedules = await response.json();
                console.log('Fetched schedules:', schedules);

                if (Array.isArray(schedules)) {
                    displaySchedules(schedules);
                } else {
                    console.error('Error: Received data is not an array');
                }
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const fetchStaff = async (staffId) => {
        try {
            const response = await fetch(`${staffApiUrl}${staffId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staff = await response.json();
                populateStaffDropdown(staff);
                currentStaffName = staff.name;
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const populateStaffDropdown = (staff) => {
        const staffDropdown = document.getElementById('input-staff');
        staffDropdown.innerHTML = '';

        const option = document.createElement('option');
        option.value = staff.id;
        option.textContent = staff.name;
        staffDropdown.appendChild(option);
        staffDropdown.disabled = true;
    };

    const displaySchedules = (schedules) => {
        scheduleTableBody.innerHTML = '';
        schedules.forEach(schedule => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${currentStaffName}</td>
                <td>${formatDateTime(schedule.startDateTime)}</td>
                <td>${formatDateTime(schedule.finishDateTime)}</td>
            `;
            scheduleTableBody.appendChild(row);
        });
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA');
        const formattedTime = date.toLocaleTimeString('en-GB');
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleAdd = async () => {
        const staffId = currentUser.nameid;
        const startDateTime = document.getElementById('input-start').value;
        const finishDateTime = document.getElementById('input-end').value;

        if (!startDateTime || !finishDateTime) {
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
                body: JSON.stringify({ staffId, startDateTime, finishDateTime })
            });

            if (response.ok) {
                alert('Schedule was added successfully!');
                await fetchSchedules(staffId);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    addButton.addEventListener('click', handleAdd);

    const token = getToken();
    if (token) {
        currentUser = decodeToken(token);
        if (currentUser) {
            await fetchStaff(currentUser.nameid);
            await fetchSchedules(currentUser.nameid);
        }
    }
});