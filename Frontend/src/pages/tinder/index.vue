<template>
  <template v-if="!dataIsReady">
    <Spinner />
  </template>
  <template v-else>
    <div id="card-wrapper" @touchstart.capture.prevent="handleSwipe" @touchmove.capture.prevent="handleSwipe"
      @touchend.capture="handleSwipe" @mousedown.capture.prevent="handleMouse" @mousemove.capture.prevent="handleMouse"
      @mouseup.capture="handleMouse">
      <div class="stack-wrapper">
        <NameplateCard :nameInfo="nameplate.names[nameplate.nextIndex]" :key="'old-' + nameplate.nextIndex"
          :style="nextCardStyle">
        </NameplateCard>
        <NameplateCard :nameInfo="nameplate.names[nameplate.currentIndex]" :key="'current-' + nameplate.currentIndex"
          :style="currentCardStyle">
        </NameplateCard>
      </div>
    </div>
  </template>

</template>

<script setup lang="ts">
import Spinner from '@/components/spinner.vue';
import NameplateCard from './components/NameplateCard.vue';
import { state, nameQuery, nameplate } from '@scripts/state.ts';
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router'
import * as api from '@scripts/api.ts';
import {Reaction, stringifyReactionType} from '@scripts/reactionType.ts';

import { useSwipe, SwipeDirection, } from '@/scripts/useSwipe';
import type { Position } from '@/scripts/useSwipe';
const dataIsReady = ref(false)
onMounted(async () => {
  console.log('Componenten laddades in, walla kör din logik här');
  await fetchData(false);
  dataIsReady.value = true;
});
onBeforeRouteLeave(async () => {
  console.log('LEAVING PAGE')
  await api.postReactions();
  nameplate.names = [];
})

const router = useRouter();

const styleDeafult = {
  rotation: 0,
  scale: 1,
  opacity: 1,
}
const currentCardStyle = computed(() => {
  if (!shouldApplyOffsets.value) return null;
  return {
    transform: `
      translate(${SwipePosition.offsetX}px, ${SwipePosition.offsetY}px)
      rotate(${nameplate.rotation}deg)
      scale(${nameplate.scale})
    `,
    opacity: nameplate.opacity,
  };
});
const shouldApplyOffsets = computed(() => {
  const dir = CurrentSwipeDirection.value;
  console.log('dir:', dir, CurrentSwipeDirection)
  return typeof dir === 'string' && !dir.includes('from');
});
const nextCardStyle = computed(() => {
  return { opacity: nameplate.nextCardOpacity }

});
//fetchdata should be done on card page load not here
const fetchData = async (isStartRequest = true) => {
  const letter = nameQuery.letters[nameQuery.currentIndex]
  try {
    console.log('MINCOUNT:',nameQuery.minCount)
    const response = await api.getNamesByLetter(letter, nameQuery.pagenum, nameQuery.minCount); // Byt ut med din API-URL
    if (isStartRequest) {
      nameplate.names = [...response]
    } else {
      nameplate.names.push(...response);
    }
    console.log("HERE: ", nameplate.names, response); // Sätt datan i din names array

    //move to card page
    router.push('/card');
  } catch (error) {
    console.error('API-anropet misslyckades:', error);
  }
};
const triggerSwipe = async (direction: SwipeDirection) => {
  const nameInfoId = nameplate.names[nameplate.currentIndex].nameInfoId
  switch (direction) {
    case 'from-up':
      router.push({ name: 'list', params: { list: stringifyReactionType(Reaction.FAVORITE) } })
      break;
    case 'up':
      addReaction(nameInfoId, Reaction.FAVORITE);
      break;
    case 'from-down':
      router.push('/menu');
      break;
    case 'down':
      break;
    case 'from-left':
      router.push({ name: 'list', params: { list: stringifyReactionType(Reaction.DISLIKE) } })
      break;
    case 'left':
      addReaction(nameInfoId, Reaction.DISLIKE);
      break;
    case 'from-right':
      router.push({ name: 'list', params: { list: stringifyReactionType(Reaction.LIKE) } })
      break;
    case 'right':
      addReaction(nameInfoId, Reaction.LIKE);
      break;
  }

  if (!direction.includes('from')) {
    nameplate.names.shift();
    if (nameplate.names.length <= 2) {
      nameQuery.pagenum++;
      await api.postReactions();
      await fetchData(false);

    }
    console.log(nameplate.names)
    if (nameplate.names.length > 1) {
      nameplate.nextIndex = nameplate.currentIndex + 1;
    } else {
      nameplate.nextIndex = 0;
    }
    if (nameplate.names.length === 0) {
      //??? next page?
      //state.display = Display.CARD;
    }
    console.log('next:', nameplate.nextIndex, 'current:', nameplate.currentIndex, nameplate.names.length)
  }
};


const addReaction = (nameInfoId: number, reaction: number) => {
  state.reactionsToAdd.push({ nameInfoId, reaction });
};

const onEndWrapper = (direction: SwipeDirection | null) => {
  if (direction === null) return;
  triggerSwipe(direction).catch(e => {
    console.error('Swipe-fuckup:', e);
  });
};

const setStylingParams = (position: Position) => {
  nameplate.rotation = position.offsetX * 0.01;
  //console.log(nameplate, nameplate.rotation);
  nameplate.scale = 1 - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 1000;
  //calculate opacity based on how far the card has moved
  nameplate.opacity = styleDeafult.opacity - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 800;
  const totalOffset = Math.abs(position.offsetX) + Math.abs(position.offsetY);
  nameplate.nextCardOpacity = Math.min(totalOffset, 200) / 200;
};
const resetStylingParams = () => {
  nameplate.rotation = 0;
  nameplate.opacity = styleDeafult.opacity;
  nameplate.scale = 1;
  nameplate.nextCardOpacity = 0;
  //console.log('opacity value RESET:', nameplate.nextCardOpacity)
};
const preCheckValues = () => {
  if (nameplate.names.length === 0 || nameplate.names[nameplate.currentIndex] === undefined) {
    console.warn("Ingen data i names ännu eller fel index.");
    return;
  }
  if (nameplate.currentIndex < nameplate.names.length - 1) {
    nameplate.nextIndex = nameplate.currentIndex + 1;
  } else {
    nameplate.nextIndex = 0;
  }
}
const { handleMouse, handleSwipe, CurrentSwipeDirection, SwipePosition } = useSwipe({
  beforeStart: preCheckValues,
  onUpdate: setStylingParams,
  onEnd: onEndWrapper,
  onEndReset: resetStylingParams
});
</script>

<style scoped>
#card-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100dvh;
  width: 100dvw;
}
</style>