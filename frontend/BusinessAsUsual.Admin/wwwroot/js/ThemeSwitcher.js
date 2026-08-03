// === ThemeSwitcher.js ===
(function() {
    function initThemeSwitcher() {
        // Support both ID formats: "theme-toggle" (main layout) and "themeToggle" (splash layout)
        const themeToggle = document.getElementById("theme-toggle") || document.getElementById("themeToggle");

        if (!themeToggle) {
            console.warn("[ThemeSwitcher] Theme toggle element not found");
            return;
        }

        // Load saved theme on page load
        const savedTheme = localStorage.getItem("selectedTheme") || "blue";
        themeToggle.value = savedTheme;
        document.body.className = ""; // Clear existing theme
        document.body.classList.add(`theme-${savedTheme}`);

        // Remove any existing listeners to prevent duplicates
        const newToggle = themeToggle.cloneNode(true);
        themeToggle.parentNode.replaceChild(newToggle, themeToggle);

        newToggle.addEventListener("change", function () {
            const selectedTheme = newToggle.value;
            document.body.className = ""; // Clear existing theme
            document.body.classList.add(`theme-${selectedTheme}`);
            localStorage.setItem("selectedTheme", selectedTheme);
            console.log(`[ThemeSwitcher] Theme set to: ${selectedTheme}`);
        });
    }

    // Run when DOM is ready
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initThemeSwitcher);
    } else {
        // DOM already loaded
        initThemeSwitcher();
    }
})();
// trigger push
