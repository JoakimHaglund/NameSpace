import { SwipeDirection, swipeHelpers} from './useSwipeHelpers.ts';
import type { Position } from './useSwipeHelpers.ts';
import { ref } from 'vue';

export function useSwipe(options: {
    beforeStart?: () => void;
    onUpdate?: (pos: Position) => void;
    onEnd?: (direction: SwipeDirection | null) => void;
    onEndReset?: () => void;
}) {

    const helper = swipeHelpers();
    const direction = ref<SwipeDirection | null>(null);

    const handleSwipe = (event: TouchEvent) => {
        event.preventDefault();
        options.beforeStart?.();
        const touch = event.touches?.[0] || event.changedTouches?.[0];

        if (event.type === "touchstart") {
            helper.setStartPos(touch);
        } else if (event.type === "touchmove") {
            helper.setOffsetPos(touch);
            direction.value = helper.getDirection();
           // console.log('Swipe direction:', direction.value, 'Position:', helper.position.offsetX, helper.position.startX);
            if (direction.value != null && !direction.value.includes('from')) {
                options.onUpdate?.(helper.position)
            }
        } else if (event.type === "touchend") {
            direction.value = helper.getDirection();
            const passedThreshold = helper.reachesThreshold();

            if (passedThreshold && direction != null) {
                options.onEnd?.(direction.value);
            }
            helper.resetOffset();
            options.onEndReset?.();
        }
    };
    
    let isMouseDown = false;
    const handleMouse = (event: MouseEvent) => {
         if (event.type === 'mousedown') isMouseDown = true;
         if (event.type === 'mouseup') isMouseDown = false;
         if (event.type === 'mousemove' && !isMouseDown) return;

        const fakeTouch = {
            clientX: event.clientX,
            clientY: event.clientY
        };

        const fakeEvent = {
            type: event.type === 'mousedown' ? 'touchstart'
                : event.type === 'mousemove' ? 'touchmove'
                : 'touchend',
            preventDefault: () => { },
            touches: [fakeTouch],
            changedTouches: [fakeTouch],
        } as unknown as TouchEvent;

        handleSwipe(fakeEvent);
    };


    return {handleMouse, handleSwipe, CurrentSwipeDirection: direction.value, SwipePosition: helper.position }
}

