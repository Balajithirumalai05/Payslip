
document.addEventListener("DOMContentLoaded", function () {
    const timesheetForm = document.getElementById('timesheetForm');
    const tableBody = document.getElementById('timesheetTableBody');
    const totalHoursElement = document.getElementById('totalHours');
    const confirmModal = document.getElementById('confirmModal');
    const confirmSubmitBtn = document.getElementById('confirmSubmit');
    const cancelSubmitBtn = document.getElementById('cancelSubmit');
    let lastSubmittedData = null;

    function formatDate(dateObj) {
        const day = String(dateObj.getDate()).padStart(2, '0');
        const month = dateObj.toLocaleString('en-US', { month: 'short' });
        const year = dateObj.getFullYear();
        return `${day}-${month}-${year}`;
    }

    function calculateTotalHours() {
        const hoursInputs = document.querySelectorAll('.hoursInput');
        let total = 0;
        hoursInputs.forEach(input => {
            total += parseFloat(input.value || 0);
        });
        totalHoursElement.textContent = `${total}`;
    }

    function addRow(existingDate, insertAfterRow = null) {
        const row = document.createElement('tr');

        row.innerHTML = `
<td class="px-4 py-2">${existingDate}</td>
<input type="hidden" name="row_date[]" value="${existingDate}">
<td class="px-4 py-2">
    <select name="mode[]" class="p-1 border rounded">
        <option value="wfo">WFO</option>
        <option value="wfh">WFH</option>
        <option value="an-lv">Annual Leave</option>
        <option value="si-lv">Sick Leave</option>
        <option value="pb-lv">Public Leave</option>
        <option value="sp-lv">Special Leave</option>
    </select>
</td>
<td class="px-4 py-2">
    <select name="location[]" class="p-1 border rounded">
        <option value="in-tn">IN-TN</option>
        <option value="in-kn">IN-KN</option>
    </select>
</td>
<td class="px-4 py-2">
    <input type="number" name="hours[]" class="hoursInput p-1 border rounded w-full" step="0.5" min="0" max="24" value="0" oninput="calculateTotalHours()">
</td>
<td class="px-4 py-2">
    <select name="billable[]" class="p-1 border rounded">
        <option value="yes">Yes</option>
        <option value="no">No</option>
    </select>
</td>
<td class="px-4 py-2">
    <input name="project_name[]" class="p-1 border rounded w-full" placeholder="Project Name">
</td>
<td class="px-4 py-2">
    <input type="text" name="client_name[]" class="p-1 border rounded w-full" placeholder="Client Name">
</td>
<td class="px-4 py-2">
    <textarea name="notes[]" class="p-1 border rounded w-full" placeholder="Enter notes"></textarea>
</td>

<td class="px-4 py-2">
        <button type="button" class="addRowBtn px-3 py-1 bg-green-500 text-white rounded-full hover:bg-green-600" 
        title="Clicking + button generates more rows to add multiple projects.">+</button>
</td>
`;

        if (insertAfterRow) {
            insertAfterRow.insertAdjacentElement("afterend", row);
        } else {
            tableBody.appendChild(row);
        }

        row.querySelector('.addRowBtn').addEventListener('click', function () {
            addRow(existingDate, row);
        });

        row.querySelector('.hoursInput').addEventListener('input', calculateTotalHours);
    }

    document.getElementById('timesheetForm').addEventListener('submit', function (event) {
        event.preventDefault();
        lastSubmittedData = new FormData(this);
        confirmModal.classList.remove('hidden'); // Show confirmation modal
    });

    // If user confirms submission
    confirmSubmitBtn.addEventListener('click', async function () {
        confirmModal.classList.add('hidden'); // Hide confirmation modal

        try {
            const response = await fetch("/timesheet/", {
                method: "POST",
                body: lastSubmittedData,
            });

            const result = await response.json();
            alert(result.message);
        } catch (error) {
            console.error('Error:', error);
            alert('An error occurred while submitting the form.');
        }
    });

    // If user cancels submission
    cancelSubmitBtn.addEventListener('click', function () {
        confirmModal.classList.add('hidden'); // Hide confirmation modal
    });

    const currentWeekStart = new Date();
    currentWeekStart.setDate(currentWeekStart.getDate() - ((currentWeekStart.getDay() + 6) % 7));
    document.getElementById('weekStartDate').valueAsDate = currentWeekStart;

    for (let i = 0; i < 5; i++) {
        let date = new Date(currentWeekStart);
        date.setDate(currentWeekStart.getDate() + i);
        addRow(formatDate(date));
    }
});
function getCurrentWeekStartDate() {
    const today = new Date();
    const monday = new Date(today);
    monday.setDate(today.getDate() - ((today.getDay() + 6) % 7));
    return monday;
}

const currentWeekStart = getCurrentWeekStartDate();
document.getElementById('weekStartDate').valueAsDate = currentWeekStart;
populateTable(currentWeekStart);

document.getElementById('weekStartDate').addEventListener('change', function () {
    populateTable(new Date(this.value));
});

document.getElementById('currentWeekButton').addEventListener('click', () => {
    document.getElementById('weekStartDate').valueAsDate = currentWeekStart;
    populateTable(currentWeekStart);
});


