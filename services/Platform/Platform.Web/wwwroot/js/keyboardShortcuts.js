// Keyboard shortcut handler for Platform module pages
export function initialize(dotNetHelper) {
    console.log('🎹 Keyboard shortcuts initialized');

    // Handle g-based navigation (g then another key)
    let gKeyPressed = false;
    let gKeyTimeout = null;

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
        // Single key shortcuts (no modifiers except maybe Shift for ?)
        else if (!e.metaKey && !e.ctrlKey && !e.altKey) {
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
                    if (!e.shiftKey) { // Only plain / without shift
                        e.preventDefault();
                        shortcut = 'slash';
                    }
                    break;
                case '?':
                    // ? is Shift+/ so shiftKey will be true
                    e.preventDefault();
                    shortcut = 'question';
                    break;
                case 'Escape':
                    shortcut = 'escape';
                    break;
                case 'g':
                    if (!e.shiftKey) { // Only plain g without shift
                        // Check if this is the second 'g' in a sequence
                        if (gKeyPressed) {
                            console.log('🔤 Second "g" pressed');
                            clearTimeout(gKeyTimeout);
                            gKeyPressed = false;
                            // Could use g-g for something, but for now just reset
                        } else {
                            // First 'g' pressed - start waiting for second key
                            console.log('🔤 First "g" pressed - waiting for second key...');
                            e.preventDefault();
                            gKeyPressed = true;
                            gKeyTimeout = setTimeout(() => {
                                console.log('⏱️ g-navigation timeout - resetting');
                                gKeyPressed = false;
                            }, 1000);
                        }
                        return; // Don't process as a normal shortcut
                    }
                    break;
                default:
                    // Handle second key after 'g'
                    if (gKeyPressed && !e.shiftKey) {
                        console.log('🔤 Second key after g:', e.key);
                        clearTimeout(gKeyTimeout);
                        gKeyPressed = false;

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
                            console.log('🎯 g-navigation shortcut:', shortcut);
                            e.preventDefault();
                        } else {
                            console.log('❌ No g-navigation match for:', e.key);
                        }
                    }
                    break;
            }
        }

        if (shortcut) {
            console.log('⌨️ Shortcut triggered:', shortcut);
            await dotNetHelper.invokeMethodAsync('HandleShortcut', shortcut);
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
