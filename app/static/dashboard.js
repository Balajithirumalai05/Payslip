// async function fetchEmployeeProfile() {
//     try {
//         const response = await fetch("/api/profile", {
//             method: "GET",
//             credentials: "include", // Include cookies for authentication
//         });
//         console.log(response);
//         if (!response.ok) {
//             throw new Error("Failed to fetch profile");
//         }

//         // const employeeData = await response.json(); // ✅ Read response only once
//         const employeeData = response
//         console.log("📢 Fetched profile data:", employeeData.name); // Debugging

//         // Function to safely update elements
//         function updateElementText(id, value) {
//             const element = document.getElementById(id);
//             if (element) {
//                 element.textContent = value || "N/A";
//             }
//         }

//         // ✅ Update text fields
//         updateElementText('employeeName', employeeData.name);
//         updateElementText('employeeRole', employeeData.role);
//         updateElementText('employeeAge', employeeData.age);
//         updateElementText('employeeDOB', employeeData.date_of_birth);
//         updateElementText('employeeDOJ', employeeData.date_of_join);
//         updateElementText('employeePhone', employeeData.phone_number);
//         updateElementText('employeeEmail', employeeData.email);
//         updateElementText('workingDays', employeeData.days_in_company);
//         updateElementText('leaveTaken', employeeData.leave_taken);
//         updateElementText('activeProjects', employeeData.active_projects);
//         updateElementText('performance', (employeeData.performance || "0") + "%");
//         updateElementText('workDays', employeeData.working_days);

//         // ✅ Update profile image
//         if (employeeData.image_url && employeeData.image_url.trim() !== "") {
//             document.getElementById('profileImage').src = `/uploads/${employeeData.image_url}`;
//         }

//         const holidaysElement = document.getElementById('holidays');
//         if (holidaysElement && employeeData.holidays) {
//             if (employeeData.holidays.length > 0) {
//                 holidaysElement.innerHTML = employeeData.holidays.map(holiday => 
//                     `<span class="block">${holiday.name} (${new Date(holiday.date).toLocaleDateString()})</span>`
//                 ).join("");
//             }
//         }

//         const birthdayElement=document.getElementById('birthdays');
//         if(birthdayElement&&employeeData.birthdays){
//             if(employeeData.birthdays.length>0){
//                 birthdayElement.innerHTML=employeeData.birthdays.map(birthdays=>
//                     `<span class="block">${birthday.name} (${new Date(birthday.date_of_birth).toLocaleDateString()})</span>`).join();
//             }
//         }

//     } catch (error) {
//         console.error("❌ Error fetching profile:", error);
//         document.getElementById("holidays").textContent = "Failed to load holidays.";
//     }
// }

// // ✅ Fetch employee profile when the page loads
// fetchEmployeeProfile();
