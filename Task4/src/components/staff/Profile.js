document.addEventListener('DOMContentLoaded', async function() {
    const apiUrl = 'https://localhost:7206/api/Staff/';
    const staffInfoContainer = document.getElementById('staff-info');
    const editButton = document.getElementById('edit-button');
    const saveButton = document.getElementById('save-button');

    const getToken = () => localStorage.getItem('token');

    const getUserData = () => {
        const userData = JSON.parse(localStorage.getItem('userData'));
        if (!userData || !userData.nameid) {
            console.error('Error: User data not found in localStorage');
            return null;
        }
        return userData;
    };

    const fetchStaffInfo = async () => {
        const userData = getUserData();
        if (!userData) return null;

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return null;
        }

        try {
            const response = await fetch(`${apiUrl}${userData.nameid}`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const staffData = await response.json();
                displayStaffInfo(staffData);
                return staffData;
            } else {
                const error = await response.text();
                console.error('Error:', error);
                return null;
            }
        } catch (error) {
            console.error('Error:', error.message);
            return null;
        }
    };

    const displayStaffInfo = (staff) => {
        staffInfoContainer.innerHTML = `
            <p><strong>Name:</strong> <span id="staff-name">${staff.name}</span></p>
            <p><strong>Position:</strong> <span id="staff-position">${staff.position}</span></p>
            <p><strong>Hourly Salary:</strong> <span id="staff-salary">${staff.hourlySalary}</span></p>
            <p><strong>Phone:</strong> <span id="staff-phone">${staff.phone}</span></p>
            <p><strong>Attendance Card:</strong> <span id="staff-card">${staff.attendanceCard}</span></p>
            <p><strong>Login:</strong> <span id="staff-login">${staff.login}</span></p>
        `;
    };

    const enableEditing = (staff) => {
        staffInfoContainer.innerHTML = `
            <p><strong>Name:</strong> <input type="text" id="edit-name" value="${staff.name}"></p>
            <p><strong>Position:</strong> <input type="text" id="edit-position" value="${staff.position}"></p>
            <p><strong>Hourly Salary:</strong> <input type="number" id="edit-salary" value="${staff.hourlySalary}"></p>
            <p><strong>Phone:</strong> <input type="number" id="edit-phone" value="${staff.phone}"></p>
            <p><strong>Attendance Card:</strong> <input type="number" id="edit-card" value="${staff.attendanceCard}"></p>
            <p><strong>Login:</strong> <input type="text" id="edit-login" value="${staff.login}"></p>
        `;
    };

    const handleEdit = () => {
        editButton.style.display = 'none';
        saveButton.style.display = 'block';
    };

    const handleSave = async () => {
        const userData = getUserData();
        if (!userData) return;

        const token = getToken();
        if (!token) {
            console.error('Error: Token not found in localStorage');
            return;
        }

        const updatedStaff = {
            name: document.getElementById('edit-name').value,
            position: document.getElementById('edit-position').value,
            hourlySalary: document.getElementById('edit-salary').value,
            phone: document.getElementById('edit-phone').value,
            attendanceCard: document.getElementById('edit-card').value,
            login: document.getElementById('edit-login').value
        };

        try {
            const response = await fetch(`${apiUrl}${userData.nameid}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updatedStaff)
            });

            if (response.ok) {
                const updatedStaffData = await response.json();
                displayStaffInfo(updatedStaffData);
                editButton.style.display = 'block';
                saveButton.style.display = 'none';
                alert('Information updated successfully!');
            } else {
                const error = await response.text();
                console.error('Error:', error);
            }
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const staffData = await fetchStaffInfo();

    editButton.addEventListener('click', () => {
        enableEditing(staffData);
        handleEdit();
    });

    saveButton.addEventListener('click', handleSave);
});