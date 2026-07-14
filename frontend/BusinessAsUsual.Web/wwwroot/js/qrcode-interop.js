// QR code rendering interop for the "Get the App" pages.
// Uses the davidshimjs/qrcodejs library loaded from a CDN in index.html.
// Exposes a small API on window.bauQr consumed from Blazor via IJSRuntime.
window.bauQr = {
    /**
     * Render a QR code for the given text into the element with the given id.
     * Waits (polls) for the qrcodejs library and the target element to be ready,
     * so it works even if OnAfterRender fires before the CDN script has loaded.
     * @param {string} elementId - target container element id
     * @param {string} text - the URL/text to encode
     * @param {number} size - width/height in px (default 220)
     */
    render: function (elementId, text, size) {
        return new Promise(function (resolve) {
            if (!text) {
                resolve(false);
                return;
            }

            var attempts = 0;
            var maxAttempts = 50; // ~5s at 100ms

            var tryRender = function () {
                var el = document.getElementById(elementId);
                var libReady = typeof QRCode !== "undefined";

                if (el && libReady) {
                    el.innerHTML = "";
                    new QRCode(el, {
                        text: text,
                        width: size || 220,
                        height: size || 220,
                        colorDark: "#1a1a2e",
                        colorLight: "#ffffff",
                        correctLevel: QRCode.CorrectLevel.M
                    });
                    resolve(true);
                    return;
                }

                if (++attempts >= maxAttempts) {
                    resolve(false);
                    return;
                }

                setTimeout(tryRender, 100);
            };

            tryRender();
        });
    }
};
