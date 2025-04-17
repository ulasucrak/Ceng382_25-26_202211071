// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

//CHATGPT Prompt projeye dark mode nasil ekleyebilirim

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("darkModeToggle");
    const body = document.body;

    function setIcon(isDark) {
        toggleBtn.textContent = isDark ? "🌞" : "🌙";
    }

    // Load from localStorage
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-mode");
        setIcon(true);
    } else {
        setIcon(false);
    }

    // Toggle on click
    toggleBtn?.addEventListener("click", function () {
        const isDark = body.classList.toggle("dark-mode");
        localStorage.setItem("theme", isDark ? "dark" : "light");
        setIcon(isDark);
    });
});
