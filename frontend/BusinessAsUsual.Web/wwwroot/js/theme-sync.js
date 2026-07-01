// Theme synchronization for iframe modules
window.ThemeSync = {
    currentTheme: { themeName: 'light', isDarkMode: false },

    // Broadcast theme changes to all iframes
    broadcastTheme: function(themeName, isDarkMode) {
        this.currentTheme = { themeName, isDarkMode };
        console.log('[ThemeSync] Broadcasting theme:', themeName, 'isDarkMode:', isDarkMode);

        const iframes = document.querySelectorAll('iframe');
        iframes.forEach(iframe => {
            try {
                iframe.contentWindow?.postMessage({
                    type: 'theme-change',
                    themeName: themeName,
                    isDarkMode: isDarkMode
                }, '*');
            } catch (e) {
                console.warn('[ThemeSync] Failed to post message to iframe:', e);
            }
        });
    },

    // Listen for theme requests from iframes
    initialize: function() {
        window.addEventListener('message', (event) => {
            if (event.data && event.data.type === 'request-theme') {
                console.log('[ThemeSync] Iframe requesting current theme');
                if (event.source) {
                    event.source.postMessage({
                        type: 'theme-change',
                        themeName: this.currentTheme.themeName,
                        isDarkMode: this.currentTheme.isDarkMode
                    }, '*');
                }
            }

            if (event.data && event.data.type === 'iframe-navigation') {
                const route = event.data.route;
                const moduleKey = event.data.moduleKey || null;

                console.log('[ThemeSync] 🎯 Iframe navigation intercepted:', route);
                console.log('[ThemeSync] Module key:', moduleKey);

                // Extract module key from current URL if not provided
                let targetModuleKey = moduleKey;
                if (!targetModuleKey) {
                    const currentPath = window.location.pathname;
                    const moduleMatch = currentPath.match(/^\/modules\/([^\/]+)/);
                    targetModuleKey = moduleMatch ? moduleMatch[1] : null;
                }

                if (targetModuleKey) {
                    // Build the new parent URL
                    const cleanRoute = route.startsWith('/') ? route.substring(1) : route;
                    const newUrl = `/modules/${targetModuleKey}/${cleanRoute}`;

                    console.log('[ThemeSync] 📍 Updating parent URL to:', newUrl);

                    // Update browser URL WITHOUT reloading the page
                    window.history.pushState(null, '', newUrl);

                    // Trigger Blazor navigation if needed (this keeps Blazor router in sync)
                    if (window.blazorNavigate) {
                        window.blazorNavigate(newUrl);
                    }
                } else {
                    console.warn('[ThemeSync] Could not determine module key for navigation');
                }

                // Trigger a custom event that Blazor can listen to
                const navEvent = new CustomEvent('iframe-navigation', { 
                    detail: { route: route, moduleKey: targetModuleKey } 
                });
                window.dispatchEvent(navEvent);
            }

            if (event.data && event.data.type === 'navigate-parent') {
                console.log('[ThemeSync] Iframe requested parent navigation to:', event.data.route);
                // Navigate the parent window
                window.location.href = event.data.route;
            }
        });
        console.log('[ThemeSync] Listening for theme requests, navigation, and parent navigation from iframes');
    }
};

// Auto-initialize when loaded as script
window.ThemeSync.initialize();

// Global sidebar reference for navigation updates
window.sidebarRef = null;

window.registerSidebarNavigationHandler = function(dotNetRef) {
    window.sidebarRef = dotNetRef;
    console.log('[ThemeSync] Sidebar navigation handler registered');
};

window.addEventListener('iframe-navigation', (event) => {
    if (window.sidebarRef) {
        window.sidebarRef.invokeMethodAsync('OnIframeNavigation', event.detail.route);
    }
});

console.log('[ThemeSync] Loaded in parent shell');
