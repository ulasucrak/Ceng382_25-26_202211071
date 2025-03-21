/*Can you provide two separate, optimized vanilla JavaScript functions—one that updates a live clock 
every second and one that toggles the visibility of all HTML form elements when the 'H' key is pressed? 
Please make sure the functions are decoupled, self-contained, and optimized for performance, and that 
they can be imported separately.
*/

// document.addEventListener('DOMContentLoaded', function () {
//     function updateClock() {
//       const now = new Date();
//       const hours = now.getHours().toString().padStart(2, '0');
//       const minutes = now.getMinutes().toString().padStart(2, '0');
//       const seconds = now.getSeconds().toString().padStart(2, '0');
//       const clockElement = document.getElementById('live-clock');
//       if (clockElement) {
//         clockElement.innerText = `${hours}:${minutes}:${seconds}`;
//       }
//     }
  
//     updateClock();
//     setInterval(updateClock, 1000);
  
//     // Function to toggle form visibility when 'H' key is pressed
//     document.addEventListener('keydown', function (event) {
//       if (event.key.toLowerCase() === 'h') {
//         document.querySelectorAll('form').forEach((form) => {
//           form.style.display = form.style.display === 'none' ? 'block' : 'none';
//         });
//       }
//     });
//   });
  
// const credentials = [];

// const loginForm = document.querySelector('form');


// loginForm.addEventListener('submit', function(event) {
//   event.preventDefault();
  
//   //How can i get email and password with js for this code
//   const email = document.querySelector('input[type="email"]').value;
//   const password = document.querySelector('input[type="password"]').value;
  
//   // Store the credentials as an object in the array
//   credentials.push({ email, password });
  
//   // Print the updated array to the console
//   console.log("Stored credentials:", credentials);
// });


document.addEventListener("DOMContentLoaded", function() {
  // ---------- Login Page Functionality ----------
  const loginForm = document.querySelector('#login form');
  if (loginForm) {
    loginForm.addEventListener("submit", function(e) {
      e.preventDefault();
      // Get the username and password from the first two inputs in the form
      const username = loginForm.querySelector("input[type='text']").value;
      const password = loginForm.querySelector("input[type='password']").value;
      
      // Check default credentials
      if (username === "admin" && password === "admin") {
        // Redirect to table.html if credentials are valid
        window.location.href = "table.html";
      } else {
        alert("Invalid credentials. Please try again.");
      }
    });
  }

  // ---------- Table Page Functionality ----------
  const classForm = document.getElementById("classForm");
  if (classForm) {
    const classTableBody = document.querySelector("#classTable tbody");
    let classes = []; // Array to store class entries

    // Handle form submission to add a new class
    classForm.addEventListener("submit", function(e) {
      e.preventDefault();
      const className = document.getElementById("className").value;
      const numPeople = document.getElementById("numPeople").value;
      const description = document.getElementById("description").value;
      const newClass = { className, numPeople, description };

      classes.push(newClass);
      addRow(newClass);
      classForm.reset();
    });

    // Function to add a new row to the table
    function addRow(classInfo) {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${classInfo.className}</td>
        <td>${classInfo.numPeople}</td>
        <td>${classInfo.description}</td>
      `;

      // --- JavaScript Events for Table Rows ---

      // 1. Row Click Event: log details and temporarily highlight the row
      row.addEventListener("click", function() {
        console.log("Row clicked:", classInfo);
        row.style.backgroundColor = "#d3d3d3";
        setTimeout(() => { row.style.backgroundColor = ""; }, 1000);
      });

      // 2. Mouseover and Mouseout Events: change background color on hover
      row.addEventListener("mouseover", function() {
        row.style.backgroundColor = "#f0f0f0";
      });
      row.addEventListener("mouseout", function() {
        row.style.backgroundColor = "";
      });

      // 3. Double-click Event: confirm and remove the row if needed
      row.addEventListener("dblclick", function() {
        if (confirm("Do you want to remove this row?")) {
          row.remove();
          // Optionally, remove from the classes array as well
        }
      });

      classTableBody.appendChild(row);
    }

    // Table Click Event: log all class entries when the table (outside rows) is clicked
    const classTable = document.getElementById("classTable");
    classTable.addEventListener("click", function(e) {
      if (e.target.tagName === "TABLE" || e.target.tagName === "THEAD" || e.target.tagName === "TBODY") {
        console.log("All class entries:", classes);
      }
    });

    // Input Focus, Blur, and Keyup Events for Real-Time Feedback and Validation
    const inputs = classForm.querySelectorAll("input, textarea");
    inputs.forEach(input => {
      input.addEventListener("focus", function() {
        input.style.border = "2px solid blue";
      });
      input.addEventListener("blur", function() {
        input.style.border = "";
        // Simple validation: if the field is empty, indicate error
        if (input.value.trim() === "") {
          input.style.border = "2px solid red";
        }
      });
      // Keyup Event: log current input value for real-time feedback
      input.addEventListener("keyup", function() {
        console.log(`Current value for ${input.id}:`, input.value);
      });
    });
  }
});
