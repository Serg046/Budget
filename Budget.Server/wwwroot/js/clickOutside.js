export function register(dotNetRef, containerRef) {
    const handler = (event) => {
        if (!containerRef.contains(event.target)) {
            dotNetRef.invokeMethodAsync('OnClickOutside');
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
