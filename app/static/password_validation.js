function validatePassword(password) {
  if (password.length < 8 || password.length > 16) return false;
  if (!/[A-Z]/.test(password)) return false;
  if (!/[a-z]/.test(password)) return false;
  if (!/\d/.test(password)) return false;
  if (!/[!@#$%^&*()]/.test(password)) return false;
  return true;
}

document.addEventListener("DOMContentLoaded", function () {
  const form = document.getElementById("changePasswordForm");

  if (form) {
    form.addEventListener("submit", function(event) {
      const newPassword = document.querySelector("input[name='new_password']").value;
      const confirmPassword = document.querySelector("input[name='confirm_password']").value;

      if (!validatePassword(newPassword)) {
        event.preventDefault();
        alert("Password must be 8-16 characters long, with at least one uppercase letter, one lowercase letter, one digit, and a special character.");
      } else if (newPassword !== confirmPassword) {
        event.preventDefault();
        alert("Passwords do not match!");
      }
    });
  } else {
    console.warn("Form with ID 'changePasswordForm' not found.");
  }
});
