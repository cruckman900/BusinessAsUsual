// Module iframe navigation helper
window.navigateModuleIframe = function (route) {
    console.log('[ModuleNav] Navigating iframe to:', route);
    const iframe = document.querySelector('.module-iframe');
    if (iframe) {
        try {
            // Get the base URL of the iframe (origin + path up to the module root)
            const iframeUrl = new URL(iframe.src);
            const newUrl = `${iframeUrl.origin}${route}`;
            console.log('[ModuleNav] New iframe URL:', newUrl);
            iframe.src = newUrl;
        } catch (e) {
            console.error('[ModuleNav] Error navigating iframe:', e);
        }
    } else {
        console.warn('[ModuleNav] No iframe found');
    }
};

console.log('[ModuleNav] Module navigation helper loaded');
