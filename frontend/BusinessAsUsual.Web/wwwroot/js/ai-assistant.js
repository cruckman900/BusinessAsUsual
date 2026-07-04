window.aiAssistant = {
    toggle: function (open) {
        const container = document.getElementById('ai-assistant-container');
        if (!container) return;
        const panel = container.querySelector('.ai-panel');
        if (open) {
            container.classList.remove('hidden');
            container.classList.add('open');
            panel.style.display = 'flex';
            this._bindEnter(panel);
            const ta = panel.querySelector('.ai-textarea');
            if (ta) setTimeout(() => ta.focus(), 60);
            this.scrollToBottom();
        } else {
            container.classList.remove('open');
            panel.style.display = 'none';
        }
    },

    // Submit on Enter (Shift+Enter inserts a newline). Bound once per textarea.
    _bindEnter: function (panel) {
        const ta = panel.querySelector('.ai-textarea');
        if (!ta || ta.dataset.enterBound) return;
        ta.dataset.enterBound = 'true';
        ta.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                const btn = panel.querySelector('.ai-send');
                if (btn && !btn.disabled) btn.click();
            }
        });
    },

    scrollToBottom: function () {
        const el = document.getElementById('ai-messages');
        if (el) setTimeout(() => { el.scrollTop = el.scrollHeight; }, 30);
    }
};
