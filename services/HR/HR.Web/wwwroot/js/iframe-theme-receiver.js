// iframe theme receiver for HR.Web module
window.IframeThemeReceiver = {
    dotNetRef: null,
    messageHandler: null,

    initialize: function (dotNetReference) {
        console.log('[IframeThemeReceiver] Initializing...');
        this.dotNetRef = dotNetReference;

        // Listen for messages from parent window
        this.messageHandler = (event) => {
            console.log('[IframeThemeReceiver] Received message:', event.data);

            if (event.data && event.data.type === 'theme-change') {
                console.log('[IframeThemeReceiver] Processing theme change:', event.data.themeName, event.data.isDarkMode);

                if (this.dotNetRef) {
                    this.dotNetRef.invokeMethodAsync('OnThemeMessage', 
                        event.data.themeName, 
                        event.data.isDarkMode);
                }
            }
        };

        window.addEventListener('message', this.messageHandler);
        console.log('[IframeThemeReceiver] Ready to receive theme updates');

        // Request current theme from parent
        if (window.parent !== window) {
            console.log('[IframeThemeReceiver] Requesting initial theme from parent');
            window.parent.postMessage({ type: 'request-theme' }, '*');
        }
    },

    // Notify parent shell about iframe navigation
    notifyNavigation: function (route) {
        if (window.parent !== window) {
            console.log('[IframeThemeReceiver] Notifying parent of navigation to:', route);
            window.parent.postMessage({ 
                type: 'iframe-navigation', 
                route: route 
            }, '*');
        }
    },

    dispose: function () {
        if (this.messageHandler) {
            window.removeEventListener('message', this.messageHandler);
        }
        this.dotNetRef = null;
        console.log('[IframeThemeReceiver] Disposed');
    }
};

console.log('[IframeThemeReceiver] Module loaded');
