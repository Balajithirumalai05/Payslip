document.addEventListener("DOMContentLoaded", async function () {
    const calendar = document.getElementById("calendar");
    const titleElement = document.getElementById("timesheet-title");
    const currentDate = new Date();
    const currentMonthIndex = currentDate.getMonth();
    const currentYear = currentDate.getFullYear();
    const monthNames = [
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    const currentMonth = monthNames[currentMonthIndex];

    if (titleElement) {
        titleElement.innerText = `Timesheet Calendar ${currentMonth} ${currentYear}`;
    } else {
        console.error("Element with ID 'timesheet-title' not found.");
    }

    try {
        const response = await fetch("/timesheets/submitted-and-unsubmitted-days");
        const data = await response.json();
        console.log("Submitted Days:", data.submitted_days);
        console.log("Unsubmitted Days:", data.unsubmitted_days);
        generateCalendar(currentYear, currentMonthIndex, data.submitted_days || [], data.unsubmitted_days || []);
    } catch (error) {
        console.error("Error fetching submitted and unsubmitted days:", error);
    }
});


function generateCalendar(year, month, submittedDays, unsubmittedDays) {
    const calendar = document.getElementById("calendar");
    calendar.innerHTML = "";

    const generateMonth = (year, month, submittedDays, unsubmittedDays) => {
        const firstDay = new Date(year, month, 1).getDay();
        const totalDays = new Date(year, month + 1, 0).getDate();
        const today = new Date();
        
        let startOfWeek = new Date(today);
        startOfWeek.setDate(today.getDate() - today.getDay()); // Start of the week (Sunday)
        let endOfWeek = new Date(today);
        endOfWeek.setDate(today.getDate() + (5 - today.getDay())); // End of the week (Saturday)

        let html = `<div class="month">
        <h3 class="text-center font-semibold mb-2">${new Date(year, month).toLocaleString('default', { month: 'long' })} ${year}</h3>
        <div class="grid grid-cols-7 gap-1">`;

        const daysOfWeek = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
        html += daysOfWeek.map(day => `<div class="text-center font-semibold text-gray-600">${day}</div>`).join("");

        for (let i = 0; i < firstDay; i++) {
            html += '<div></div>';
        }

        for (let day = 1; day <= totalDays; day++) {
            let classes = "p-2 text-center rounded-md";
            const date = new Date(year, month, day);

            if (submittedDays.some(d => d.day === day && d.month === month + 1)) {
                classes += " bg-green-400 text-white"; // Submitted - Green
            } 
            else if (date >= startOfWeek && date <= endOfWeek) {
                classes += " ring-1 ring-blue-500"; // Current week - Blue
            } 
            else if (unsubmittedDays.some(d => d.day === day && d.month === month + 1) && date.getDay() !== 0 && date.getDay() !== 6) {
                classes += " bg-red-400 text-white"; // Unsubmitted - Red (Except Sat/Sun)
            }

            if (date.toDateString() === today.toDateString()) {
                classes += " bg-blue-500 text-white"; // Today’s Date - Highlighted
            }

            html += `<div class="${classes}">${day}</div>`;
        }
        html += "</div></div>";
        return html;
    };

    let html = '<div class="calendar-horizontal" style="display: flex; gap: 20px;">';
    for (let i = -1; i < 3; i++) {
        const newMonth = (month + i + 12) % 12;
        const newYear = year + Math.floor((month + i) / 12);
        html += generateMonth(newYear, newMonth, submittedDays, unsubmittedDays);
    }
    html += '</div>';
    calendar.innerHTML = html;
}
