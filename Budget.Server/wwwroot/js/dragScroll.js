const EDGE_PERCENT = 0.2;
const SCROLL_SPEED = 20;

export function checkEdgeScroll(boardElement, clientX) {
    const rect = boardElement.getBoundingClientRect();
    const edgeSize = rect.width * EDGE_PERCENT;

    if (clientX < rect.left + edgeSize) {
        boardElement.scrollLeft -= SCROLL_SPEED;
    } else if (clientX > rect.right - edgeSize) {
        boardElement.scrollLeft += SCROLL_SPEED;
    }
}
