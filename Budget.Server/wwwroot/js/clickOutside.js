export function register(dotNetRef, containerRef) {
    const handler = (event) => {
        // Use composedPath(), not event.target/closest(): several menu items (e.g.
        // "Details", "Change group") remove themselves from the DOM as a side
        // effect of their own click (closing the menu), and by the time this
        // handler runs afterward, event.target may already be detached - making
        // containerRef.contains(event.target)/closest() see it as "outside" even
        // though the click originated inside. composedPath() is a snapshot of the
        // ancestry taken at dispatch time, so it's unaffected by that mutation.
        const path = event.composedPath();
        const hasClass = (el, className) => el.classList && el.classList.contains(className);

        // The details/change-group modals render outside containerRef (they're a
        // fixed overlay), so opening/closing them would otherwise register as an
        // "outside" click and wipe whatever selection is currently highlighted.
        const insideContainer = path.includes(containerRef);
        const insideModal = path.some(el => hasClass(el, 'modal-backdrop'));
        if (!insideContainer && !insideModal) {
            dotNetRef.invokeMethodAsync('OnClickOutside');
        }

        // The row actions menu has its own, narrower "outside" boundary (just the
        // cell it opened from), so it gets a separate check with the same handler.
        const insideActionsCell = path.some(el => hasClass(el, 'transactions-table__actions-cell'));
        if (!insideActionsCell) {
            dotNetRef.invokeMethodAsync('OnClickOutsideMenu');
        }
    };
    document.addEventListener('click', handler);
    containerRef._clickOutsideHandler = handler;
}

export function unregister(containerRef) {
    if (!containerRef) {
        return;
    }

    const handler = containerRef._clickOutsideHandler;
    if (handler) {
        document.removeEventListener('click', handler);
        delete containerRef._clickOutsideHandler;
    }
}
