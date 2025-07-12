function toggleModal() {
    const editModal = document.getElementById("request-leave");
    editModal.classList.toggle("hidden");
}

// Modify your script to correctly parse and use the leave history

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Ensure the leave history data is parsed correctly
    try {
        // Parse the leave history from the hidden div
        window.leaveHistory = JSON.parse(document.getElementById('leaveHistoryData').textContent);

        // Function to extract leave data by type
        function getLeaveDataByType(leaveType) {
            // Initialize an array for 12 months with default 0 values
            let monthlyLeaveData = new Array(12).fill(0);

            // Ensure leaveHistory exists and is an array
            if (!window.leaveHistory || !Array.isArray(window.leaveHistory)) {
                console.error("Leave history is not available or not an array.");
                return monthlyLeaveData;
            }

            // Loop through leave history and extract leave days for each month
            window.leaveHistory.forEach(leave => {
                if (leave.leave_type === leaveType) {
                    let startMonth = new Date(leave.start_date).getMonth(); // Get month index (0-11)
                    monthlyLeaveData[startMonth] += leave.leave_days;
                }
            });

            return monthlyLeaveData;
        }



        // Annual Leave Chart
        var annualLeaveChart = new Chart(document.getElementById('annualLeaveChart'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    label: 'Annual Leave',
                    data: getLeaveDataByType('Annual_leave'),
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1,
                    fill: false
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }  // ✅ Fix: Y-axis only shows whole numbers
                    }
                }
            }
        });

        // Sick Leave Chart
        var sickLeaveChart = new Chart(document.getElementById('sickLeaveChart'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    label: 'Sick Leave',
                    data: getLeaveDataByType('Sick_leave'),
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1,
                    fill: false
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }  // ✅ Fix: Y-axis only shows whole numbers
                    }
                }
            }
        });

        // Special Leave Chart
        var specialLeaveChart = new Chart(document.getElementById('specialLeaveChart'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    label: 'Special Leave',
                    data: getLeaveDataByType('Special_leave'),
                    borderColor: 'rgba(153, 102, 255, 1)',
                    borderWidth: 1,
                    fill: false
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }  // ✅ Fix: Y-axis only shows whole numbers
                    }
                }
            }
        });

        // Public Leave Chart
        var publicLeaveChart = new Chart(document.getElementById('publicLeaveChart'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    label: 'Public Leave',
                    data: getLeaveDataByType('Public_leave'),
                    borderColor: 'rgba(255, 159, 64, 1)',
                    borderWidth: 1,
                    fill: false
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { stepSize: 1 }  // ✅ Fix: Y-axis only shows whole numbers
                    }
                }
            }
        });

        console.log("Annual Leave Data:", getLeaveDataByType('Annual_leave'));
        console.log("Sick Leave Data:", getLeaveDataByType('Sick_leave'));
        console.log("Special Leave Data:", getLeaveDataByType('Special_leave'));
        console.log("Public Leave Data:", getLeaveDataByType('Public_leave'));
        console.log("Full Leave History:", window.leaveHistory);

    } catch (error) {
        console.error("Error processing leave history:", error);
    }
});

//leave apply validation
document.addEventListener('DOMContentLoaded', function () {
    const startInput = document.getElementById('start_date');
    const endInput = document.getElementById('end_date');

    // Set min end date when start date changes
    startInput.addEventListener('change', function () {
        const startDateValue = startInput.value;
        endInput.value = ''; // Reset previous selection
        endInput.min = startDateValue; // Restrict end date to start date or later
    });

    // Form validation
    document.getElementById('leave-check').addEventListener('submit', function (event) {
        const startDate = new Date(startInput.value);
        const endDate = new Date(endInput.value);

        if (endDate < startDate) {
            event.preventDefault();
            alert('End date cannot be earlier than start date.');
        }
    });
});
