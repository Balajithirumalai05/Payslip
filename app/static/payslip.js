// let currentYear = null;
// let currentMonth = null;

// async function loadYears() {
//     resetView();
//     try {
//         const res = await fetch('/api/payslips/years');
//         const years = await res.json();
//         document.getElementById('yearButtons').innerHTML = years.map(year => `
//     <button class="px-6 py-1.5 bg-primary text-white rounded-xl" onclick="loadMonths('${year}')">${year}</button>
//     `).join('');
//     } catch (error) {
//         console.error('Error loading years:', error);
//     }
// }

// async function loadMonths(year) {
//     currentYear = year;
//     resetView();
//     try {
//         const res = await fetch(`/api/payslips/${year}/months`);
//         const months = await res.json();
//         document.getElementById('monthButtons').innerHTML = months.map(month => `
//     <button class="px-4 py-2 bg-gray-500 text-white rounded" onclick="loadPDF('${year}', '${month}')">${month}</button>
//     `).join('');
//         document.getElementById('backToYears').classList.remove('hidden');
//         document.getElementById('currentSelection').textContent = `Viewing: ${year}`;
//     } catch (error) {
//         console.error('Error loading months:', error);
//     }
// }

// async function loadPDF(year, month) {
//     // currentMonth = month;
//     // resetView();
//     try {
//         // const res = await fetch(`/api/payslips/${year}/${month}/url`);
//         // const data = await res.json();
//         document.getElementById('pdfFrame').src = `/static/${data.url}`;
//         document.getElementById('pdfFrame').classList.remove('hidden');
//         // document.getElementById('backToMonths').classList.remove('hidden');
//         // document.getElementById('currentSelection').textContent = `Viewing: ${year} - ${month}`;
//     } catch (error) {
//         console.error('Error loading PDF:', error);
//         document.getElementById('errorMessage').classList.remove('hidden');
//     }
// }

// function backToYears() {
//     loadYears();
// }

// function backToMonths() {
//     if (currentYear) {
//         loadMonths(currentYear);
//     }
// }

// function resetView() {
//     document.getElementById('yearButtons').innerHTML = '';
//     document.getElementById('monthButtons').innerHTML = '';
//     document.getElementById('pdfFrame').classList.add('hidden');
//     document.getElementById('errorMessage').classList.add('hidden');
// }

// document.addEventListener('DOMContentLoaded', loadYears);
