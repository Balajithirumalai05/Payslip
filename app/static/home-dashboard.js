async function fetchEmployeeProfile() {
    try {
        const response = await fetch("/api/profile", {
            method: "GET",
            credentials: "include", // Include cookies for authentication
        });

        if (!response.ok) {
            throw new Error("Failed to fetch profile");
        }

        const employeeData = await response.json(); // ✅ Read response properly

        console.log("📢 Fetched profile data:", employeeData.name); // Debugging

        // Function to safely update elements
        function updateElementText(id, value) {
            const element = document.getElementById(id);
            if (element) {
                element.textContent = value || "N/A";
            }
        }

        // ✅ Update text fields
        updateElementText('employeeName', employeeData.name);
        updateElementText('employeeRole', employeeData.role);
        updateElementText('employeeAge', employeeData.age);
        updateElementText('employeeDOB', employeeData.date_of_birth);
        updateElementText('employeeDOJ', employeeData.date_of_join);
        updateElementText('employeePhone', employeeData.phone_number);
        updateElementText('employeeEmail', employeeData.email);
        updateElementText('workingDays', employeeData.days_in_company);
        updateElementText('leaveTaken', employeeData.leave_taken);
        updateElementText('activeProjects', employeeData.active_projects);
        updateElementText('performance', (employeeData.performance || "0") + "%");
        updateElementText('workDays', employeeData.working_days);

        // ✅ Update profile image
        // const profileImageElement = document.getElementById('profileImage');
        // if (profileImageElement && employeeData.image_url && employeeData.image_url.trim() !== "") {
        //     profileImageElement.src = employeeData.image_url.trim();
        // }

        // ✅ Update holidays
        const holidaysElement = document.getElementById('holidays');
        if (holidaysElement && employeeData.holidays) {
            holidaysElement.innerHTML = employeeData.holidays.map(holiday => 
                `<span class="block">${holiday.name} (${new Date(holiday.date).toLocaleDateString()})</span>`
            ).join("");
        }

        // ✅ Update birthdays
        const birthdayElement = document.getElementById('birthdays');
        if (birthdayElement && employeeData.birthdays) {
            birthdayElement.innerHTML = employeeData.birthdays.map(birthday =>
                `<span class="block">${birthday.name} (${new Date(birthday.date_of_birth).toLocaleDateString()})</span>`
            ).join("");
        }

    } catch (error) {
        console.error("❌ Error fetching profile:", error);
        document.getElementById("holidays").textContent = "Failed to load holidays.";
    }
}

// ✅ Fetch employee profile when the page loads
fetchEmployeeProfile();
