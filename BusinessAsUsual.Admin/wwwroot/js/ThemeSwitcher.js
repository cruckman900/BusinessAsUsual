// === ThemeSwitcher.js ===
document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.getElementById("theme-toggle");

    themeToggle.addEventListener("change", function () {
        const selectedTheme = themeToggle.value;
        document.body.className = ""; // Clear existing theme
        document.body.classList.add(`theme-${selectedTheme}`);
        localStorage.setItem("selectedTheme", selectedTheme);
        console.log(`[SmartCommit] Theme set to: ${selectedTheme}`);
    });
});
// trigger push