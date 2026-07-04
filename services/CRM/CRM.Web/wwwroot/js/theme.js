// Theme detection utilities for CRM Reports
window.crmTheme = {
    isDarkMode: function() {
        // Check if running in shell app with ThemeContext
        if (window.mudBlazorIsDarkMode !== undefined) {
            return window.mudBlazorIsDarkMode;
        }
        // Fallback to system preference
        return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    }
};
