/*Can you provide two separate, optimized vanilla JavaScript functions—one that updates a live clock 
every second and one that toggles the visibility of all HTML form elements when the 'H' key is pressed? 
Please make sure the functions are decoupled, self-contained, and optimized for performance, and that 
they can be imported separately.
*/

document.addEventListener('DOMContentLoaded', function () {
    function updateClock() {
      const now = new Date();
      const hours = now.getHours().toString().padStart(2, '0');
      const minutes = now.getMinutes().toString().padStart(2, '0');
      const seconds = now.getSeconds().toString().padStart(2, '0');
      const clockElement = document.getElementById('live-clock');
      if (clockElement) {
        clockElement.innerText = `${hours}:${minutes}:${seconds}`;
      }
    }
  
    updateClock();
    setInterval(updateClock, 1000);
  
    // Function to toggle form visibility when 'H' key is pressed
    document.addEventListener('keydown', function (event) {
      if (event.key.toLowerCase() === 'h') {
        document.querySelectorAll('form').forEach((form) => {
          form.style.display = form.style.display === 'none' ? 'block' : 'none';
        });
      }
    });
  });
  
const credentials = [];

const loginForm = document.querySelector('form');


loginForm.addEventListener('submit', function(event) {
  event.preventDefault();
  
  //How can i get email and password with js for this code
  const email = document.querySelector('input[type="email"]').value;
  const password = document.querySelector('input[type="password"]').value;
  
  // Store the credentials as an object in the array
  credentials.push({ email, password });
  
  // Print the updated array to the console
  console.log("Stored credentials:", credentials);
});
