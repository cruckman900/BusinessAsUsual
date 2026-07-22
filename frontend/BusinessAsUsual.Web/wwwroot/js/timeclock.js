// Simple localStorage-backed persistence for the footer TimeClock component.
window.timeClock = {
    key: 'bau.timeclock',

    load: function () {
        try {
            const raw = window.localStorage.getItem(this.key);
            return raw ? JSON.parse(raw) : null;
        } catch (e) {
            return null;
        }
    },

    save: function (state) {
        try {
            window.localStorage.setItem(this.key, JSON.stringify(state));
        } catch (e) {
            // ignore storage errors (private mode, quota, etc.)
        }
    },

    clear: function () {
        try {
            window.localStorage.removeItem(this.key);
        } catch (e) {
            // ignore
        }
    }
};
