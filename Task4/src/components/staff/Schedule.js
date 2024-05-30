document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Schedule/staff/';
    const staffApiUrl = 'https://localhost:7206/api/Staff/';
    const scheduleTableBody = document.querySelector('#schedule tbody');
    const addButton = document.querySelector('.btn-add');
    let currentUser = null;
    let currentStaffName = '';

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

    const fetchSchedules = async (staffId) => {
        const schedules = await fetchWithAuth(`${apiUrl}${staffId}`);
        if (schedules && Array.isArray(schedules)) {
            displaySchedules(schedules);
        } else {
            console.error('Error: Failed to fetch schedules');
        }
    };

    const fetchStaff = async (staffId) => {
        const staff = await fetchWithAuth(`${staffApiUrl}${staffId}`);
        if (staff) {
            populateStaffDropdown(staff);
            currentStaffName = staff.name;
        } else {
            console.error('Error: Failed to fetch staff');
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
            await fetchWithAuth(apiUrl, {
                method: 'POST',
                body: JSON.stringify({ staffId, startDateTime, finishDateTime })
            });

            alert('Schedule was added successfully!');
            await fetchSchedules(staffId);
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