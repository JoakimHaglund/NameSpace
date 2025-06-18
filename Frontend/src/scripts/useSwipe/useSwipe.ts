import { nameplate } from '@scripts/state.ts';
import * as helper from './useSwipeHelpers.ts';
import { ref } from 'vue';

export function useSwipe(options: {
    beforeStart?: () => void;
    onUpdate?: (pos: helper.Position) => void;
    onEnd?: (direction: helper.SwipeDirection | null) => void;
    onEndReset?: () => void;
    })
    {

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
    return {handleSwipe, CurrentSwipeDirection: direction, SwipePosition: helper.position}
}

