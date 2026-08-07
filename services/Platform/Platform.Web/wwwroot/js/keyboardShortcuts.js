// Keyboard shortcut handler for Platform module pages
export function initialize(dotNetHelper) {
    console.log('🎹 Keyboard shortcuts initialized');

    const handleKeyDown = async (e) => {
        let shortcut = null;

        // Alt + Key shortcuts (browser won't block these)
        if (e.altKey && !e.metaKey && !e.ctrlKey && !e.shiftKey) {
            switch (e.key.toLowerCase()) {
                case 'n':
                    e.preventDefault();
                    shortcut = 'alt-n';
                    break;
                case 's':
                    e.preventDefault();
                    shortcut = 'alt-s';
                    break;
                case 'e':
                    e.preventDefault();
                    shortcut = 'alt-e';
                    break;
                case 'f':
                    e.preventDefault();
                    shortcut = 'alt-f';
                    break;
                case 'k':
                    e.preventDefault();
                    shortcut = 'alt-k';
                    break;
            }
        }
        // Single key shortcuts (no modifiers)
        else if (!e.metaKey && !e.ctrlKey && !e.shiftKey && !e.altKey) {
            // Ignore if user is typing in an input field
            const activeElement = document.activeElement;
            if (activeElement && (
                activeElement.tagName === 'INPUT' ||
                activeElement.tagName === 'TEXTAREA' ||
                activeElement.isContentEditable
            )) {
                return;
            }

            switch (e.key) {
                case '/':
                    e.preventDefault();
                    shortcut = 'slash';
                    break;
                case '?':
                    e.preventDefault();
                    shortcut = 'question';
                    break;
                case 'Escape':
                    shortcut = 'escape';
                    break;
                case 'g':
                    // Start g-based navigation sequence
                    handleGNavigation(e, dotNetHelper);
                    return;
            }
        }

        if (shortcut) {
            console.log('⌨️ Shortcut triggered:', shortcut);
            await dotNetHelper.invokeMethodAsync('HandleShortcut', shortcut);
        }
    };

    // Handle g-based navigation (g then another key)
    let gKeyPressed = false;
    let gKeyTimeout = null;

    const handleGNavigation = (e, dotNetHelper) => {
        if (!gKeyPressed) {
            gKeyPressed = true;
            gKeyTimeout = setTimeout(() => {
                gKeyPressed = false;
            }, 1000); // Reset after 1 second
        } else {
            // Second key pressed after 'g'
            clearTimeout(gKeyTimeout);
            gKeyPressed = false;

            let shortcut = null;
            switch (e.key.toLowerCase()) {
                case 'h':
                    shortcut = 'g-h';
                    break;
                case 'd':
                    shortcut = 'g-d';
                    break;
                case 'u':
                    shortcut = 'g-u';
                    break;
                case 'r':
                    shortcut = 'g-r';
                    break;
                case 'n':
                    shortcut = 'g-n';
                    break;
                case 's':
                    shortcut = 'g-s';
                    break;
            }

            if (shortcut) {
                e.preventDefault();
                dotNetHelper.invokeMethodAsync('HandleShortcut', shortcut);
            }
        }
    };

    document.addEventListener('keydown', handleKeyDown);

    // Return cleanup function
    return {
        dispose: () => {
            document.removeEventListener('keydown', handleKeyDown);
            if (gKeyTimeout) {
                clearTimeout(gKeyTimeout);
            }
        }
    };
}
