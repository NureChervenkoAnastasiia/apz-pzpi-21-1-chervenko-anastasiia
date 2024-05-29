document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Schedule/';
    const staffApiUrl = 'https://localhost:7206/api/Staff/';
    const scheduleTableBody = document.querySelector('#schedule tbody');
    const addButton = document.querySelector('.btn-add');

    const getToken = () => localStorage.getItem('token');

    const fetchSchedules = async () => {
        try {
            const response = await fetch(apiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const schedules = await response.json();
                displaySchedules(schedules);
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const fetchStaff = async () => {
        try {
            const response = await fetch(staffApiUrl, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staff = await response.json();
                populateStaffDropdown(staff);
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

        staff.forEach(member => {
            const option = document.createElement('option');
            option.value = member.id;
            option.textContent = member.name;
            staffDropdown.appendChild(option);
        });
    };

    const displaySchedules = async (schedules) => {
        scheduleTableBody.innerHTML = '';
        for (const schedule of schedules) {
            const staffName = await getStaffNameById(schedule.staffId);
            if (!staffName) {
                console.error(`Staff member not found for schedule with ID ${schedule.id}`);
                continue;
            }

            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${staffName}</td>
                <td>${formatDateTime(schedule.startDateTime)}</td>
                <td>${formatDateTime(schedule.finishDateTime)}</td>
                <td><button class="btn-edit" data-scheduleid="${schedule.id}">Edit</button></td>
                <td><button class="btn-delete" data-scheduleid="${schedule.id}">Delete</button></td>
            `;
            scheduleTableBody.appendChild(row);
        }
    };

    const getStaffNameById = async (staffId) => {
        try {
            const response = await fetch(`${staffApiUrl}${staffId}`, {
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staff = await response.json();
                return staff.name;
            } else {
                console.error('Error:', await response.text());
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const formatDateTime = (dateTime) => {
        const date = new Date(dateTime);
        const formattedDate = date.toLocaleDateString('en-CA');
        const formattedTime = date.toLocaleTimeString('en-GB');
        return `Дата: ${formattedDate}<br>Час: ${formattedTime}`;
    };

    const handleDelete = async (scheduleId) => {
        try {
            const response = await fetch(`${apiUrl}${scheduleId}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                alert('Schedule was deleted successfully!');
                await fetchSchedules();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleEdit = (scheduleId) => {
        const row = document.querySelector(`button[data-scheduleid="${scheduleId}"]`).parentNode.parentNode;
        const [nameCell, startCell, endCell, editButtonCell, deleteButtonCell] = row.cells;

        const staffName = nameCell.textContent;
        const startDateTime = startCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');
        const finishDateTime = endCell.textContent.replace('Дата: ', '').replace(' Час: ', 'T');

        nameCell.innerHTML = `<select>${document.getElementById('input-staff').innerHTML}</select>`;
        nameCell.querySelector('select').value = staffName;
        startCell.innerHTML = `<input type="datetime-local" value="${startDateTime}">`;
        endCell.innerHTML = `<input type="datetime-local" value="${finishDateTime}">`;

        editButtonCell.innerHTML = `<button class="btn-save" data-scheduleid="${scheduleId}">Save</button>`;
        deleteButtonCell.innerHTML = `<button class="btn-cancel" data-scheduleid="${scheduleId}">Cancel</button>`;
    };

    const handleSave = async (scheduleId) => {
        const row = document.querySelector(`button[data-scheduleid="${scheduleId}"]`).parentNode.parentNode;
        const staffId = row.cells[0].querySelector('select').value;
        const startDateTime = row.cells[1].querySelector('input').value;
        const finishDateTime = row.cells[2].querySelector('input').value;

        if (!staffId || !startDateTime || !finishDateTime) {
            alert('Please fill in all fields');
            return;
        }

        try {
            const response = await fetch(`${apiUrl}${scheduleId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${getToken()}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ staffId, startDateTime, finishDateTime })
            });

            if (response.ok) {
                alert('Schedule was updated successfully!');
                await fetchSchedules();
            } else {
                console.error('Error:', await response.json());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const handleCancel = (scheduleId) => {
        fetchSchedules();
    };

    const handleAdd = async () => {
        const staffId = document.getElementById('input-staff').value;
        const startDateTime = document.getElementById('input-start').value;
        const finishDateTime = document.getElementById('input-end').value;

        if (!staffId || !startDateTime || !finishDateTime) {
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
                await fetchSchedules();
            } else {
                console.error('Error:', await response.text());
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    scheduleTableBody.addEventListener('click', function(event) {
        const target = event.target;
        const scheduleId = target.getAttribute('data-scheduleid');

        if (target.classList.contains('btn-delete')) {
            handleDelete(scheduleId);
        } else if (target.classList.contains('btn-edit')) {
            handleEdit(scheduleId);
        } else if (target.classList.contains('btn-save')) {
            handleSave(scheduleId);
        } else if (target.classList.contains('btn-cancel')) {
            handleCancel(scheduleId);
        }
    });

    addButton.addEventListener('click', handleAdd);

    await fetchStaff();
    await fetchSchedules();
});