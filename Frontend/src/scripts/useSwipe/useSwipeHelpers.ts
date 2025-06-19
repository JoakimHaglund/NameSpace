import { reactive } from 'vue';

export enum SwipeDirection {
    FROM_LEFT = 'from-left',
    LEFT = 'left',
    FROM_RIGHT = 'from-right',
    RIGHT = 'right',
    FROM_UP = 'from-up',
    UP = 'up',
    FROM_DOWN = 'from-down',
    DOWN = 'down',
};
export type Position = {
    offsetX: number,
    startX: number,
    offsetY: number,
    startY: number,
    EDGE_THRESHOLD: number,
    SWIPE_THRESHOLD: number,
};
export const position = reactive<Position>({
    offsetX: 0,
    startX: 0,
    offsetY: 0,
    startY: 0,
    EDGE_THRESHOLD: 0.09,
    SWIPE_THRESHOLD: 200,
});

export const getDirection = () => {
    const { startX, startY, offsetX, offsetY, EDGE_THRESHOLD } = position;
    const absX = Math.abs(offsetX);
    const absY = Math.abs(offsetY);
    const widthThreshold = window.innerWidth * EDGE_THRESHOLD;
    const heightThreshold = window.innerHeight * EDGE_THRESHOLD;

    const isHorizontal = absX > absY;
console.log('width:', widthThreshold, '/',window.innerWidth ,'height', heightThreshold, '/',window.innerHeight )
    const xEdgeLeft = startX <= widthThreshold;
    const xEdgeRight = startX >= window.innerWidth - widthThreshold;

    const yEdgeTop = startY <= heightThreshold;
    const yEdgeBottom = startY >= window.innerHeight - heightThreshold;

    if (isHorizontal) {
        if (offsetX < 0) return xEdgeRight ? SwipeDirection.FROM_RIGHT : SwipeDirection.LEFT;
        if (offsetX > 0) return xEdgeLeft ? SwipeDirection.FROM_LEFT : SwipeDirection.RIGHT;
    }

    if (!isHorizontal) {
        if (offsetY < 0) return yEdgeBottom ? SwipeDirection.FROM_DOWN : SwipeDirection.UP;
        if (offsetY > 0) return yEdgeTop ? SwipeDirection.FROM_UP : SwipeDirection.DOWN;
    }

    return null;
};
export const reachesThreshold = () => {
    const absX = Math.abs(position.offsetX);
    const absY = Math.abs(position.offsetY);
    const passedX = absX > position.SWIPE_THRESHOLD;
    const passedY = absY > position.SWIPE_THRESHOLD;
    const passedThreshold = passedX || passedY;
    return passedThreshold;
};
export const setStartPos = (touch: Touch) => {
    position.startX = touch.clientX;
    position.startY = touch.clientY;
};
export const setOffsetPos = (touch: Touch) => {
    position.offsetX = touch.clientX - position.startX;
    position.offsetY = touch.clientY - position.startY;
};
export const resetOffset = () => {
    position.offsetX = 0;
    position.offsetY = 0;
};