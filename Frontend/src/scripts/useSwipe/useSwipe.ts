import { nameplate } from '@scripts/state.ts';
import * as helper from './useSwipeHelpers.ts';
import { ref } from 'vue';

export function useSwipe(options: {
    beforeStart?: () => void;
    onUpdate?: (pos: helper.Position) => void;
    onEnd?: (direction: helper.SwipeDirection | null) => void;
    onEndReset?: () => void;
}) {

    const direction = ref<helper.SwipeDirection | null>(null);
    const handleSwipe = (event: TouchEvent) => {
        event.preventDefault();
        options.beforeStart?.();
        const touch = event.touches?.[0] || event.changedTouches?.[0];

        if (event.type === "touchstart") {
            helper.setStartPos(touch);
        } else if (event.type === "touchmove") {
            helper.setOffsetPos(touch);
            direction.value = helper.getDirection();

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

        // Mocka touch-event för din swipe-logik
        const fakeEvent = {
            type: event.type === 'mousedown' ? 'touchstart'
                : event.type === 'mousemove' ? 'touchmove'
                : 'touchend',
            preventDefault: () => { },
            touches: [fakeTouch],
            changedTouches: [fakeTouch],
        } as unknown as TouchEvent;

        handleSwipe(fakeEvent); // Använd samma som för touch
    };


    return {handleMouse, handleSwipe, CurrentSwipeDirection: direction, SwipePosition: helper.position }
}

