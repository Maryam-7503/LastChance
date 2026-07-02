document.addEventListener("DOMContentLoaded", () => {

    const body = document.body;
    const lightIcon = document.getElementById("lightIcon");
    const darkIcon = document.getElementById("darkIcon");
    const themeBtn = document.querySelector(".theme-btn");

    // Restore Theme
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-theme");
        lightIcon.classList.remove("active");
        darkIcon.classList.add("active");
    }

    // Toggle Theme
    themeBtn.addEventListener("click", toggleTheme);

});

function toggleTheme() {

    const body = document.body;
    const lightIcon = document.getElementById("lightIcon");
    const darkIcon = document.getElementById("darkIcon");

    body.classList.toggle("dark-theme");

    if (body.classList.contains("dark-theme")) {

        localStorage.setItem("theme", "dark");

        lightIcon.classList.remove("active");
        darkIcon.classList.add("active");

    } else {

        localStorage.setItem("theme", "light");

        lightIcon.classList.add("active");
        darkIcon.classList.remove("active");

    }
}