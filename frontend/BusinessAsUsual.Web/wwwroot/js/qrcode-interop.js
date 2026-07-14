// QR code rendering interop for the "Get the App" pages.
// Uses the davidshimjs/qrcodejs library loaded from a CDN in index.html.
// Exposes a small API on window.bauQr consumed from Blazor via IJSRuntime.
window.bauQr = {
    /**
     * Render a QR code for the given text into the element with the given id.
     * Clears any previous QR before drawing so it can be re-rendered on nav.
     * @param {string} elementId - target container element id
     * @param {string} text - the URL/text to encode
     * @param {number} size - width/height in px (default 220)
     */
    render: function (elementId, text, size) {
        var el = document.getElementById(elementId);
        if (!el) {
            return false;
        }

        // Clear any existing QR (e.g. when switching platforms).
        el.innerHTML = "";

        if (typeof QRCode === "undefined" || !text) {
            return false;
        }

        new QRCode(el, {
            text: text,
            width: size || 220,
            height: size || 220,
            colorDark: "#1a1a2e",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.M
        });

        return true;
    }
};
