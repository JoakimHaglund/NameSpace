<script setup lang="ts">
import * as api from '../scripts/api.ts';
import { state, lists , Display, Reaction, nameQuery} from '../scripts/state.ts';

import {reactive, computed} from 'vue';

const svgFileLocations = reactive({
    gender: {
    female: '../resources/svgs/genders/female.svg',
    male: '../resources/svgs/genders/male.svg',
    unknown: '../resources/svgs/genders/unknown.svg',
    },
    reaction: {
    favorite: '../resources/svgs/reactions/favorite.svg',
    liked: '../resources/svgs/reactions/liked.svg',
    disliked: '../resources/svgs/reactions/disliked.svg',
    noReaction: '../resources/svgs/reactions/no-reaction.svg',
    },
});
const styleDeafult = {
  rotation: 0,
  scale: 1,
  opacity: 1,
}
const position = reactive({
    offsetX: 0,
    startX: 0,
    offsetY: 0,
    startY: 0,
    EDGE_THRESHOLD: 120,
    SWIPE_THRESHOLD: 200,
  });
  const nameplate = reactive({
    names: [] as {
        nameInfoId: number, 
        reaction: Reaction, 
        gender: number,
        name: string,
        description: string,
        amount: number
    }[],
    currentIndex: 0,
    nextIndex: 0,
    rotation: 0,
    scale: 1,
    opacity: styleDeafult.opacity,
    nextCardOpacity: 0,
  });

const handleSwipe = (event: TouchEvent) => {
  event.preventDefault();
    if (nameplate.names.length === 0 || nameplate.names[nameplate.currentIndex] === undefined) {
      console.warn("Ingen data i names ännu eller fel index.");
      return;
    }
    if (nameplate.currentIndex < nameplate.names.length - 1) {
      nameplate.nextIndex = nameplate.currentIndex + 1;
    } else {
      nameplate.nextIndex = 0;
    }

    const touch = event.touches?.[0] || event.changedTouches?.[0];
    //console.log('touch event:', event.type);

    if (event.type === "touchstart") {
      setStartPos(touch);
    } else if (event.type === "touchmove") {
      setOffsetPos(touch);
      const direction = getDirection();
      //console.log(direction)
      if (direction != null && !direction.includes('from')) {
        setStylingParams();
        
      }
    } else if (event.type === "touchend") {
      const direction = getDirection();
      const passedThreshold = reachesThreshold();
      //console.log('Swipe direction:', direction);
      if (passedThreshold && direction != null) {
        triggerSwipe(direction);
      }
      resetOffset();
      resetStylingParams();
    }
  };
  
  const getDirection = () => {
    const { startX, startY, offsetX, offsetY, EDGE_THRESHOLD } = position;
    const absX = Math.abs(offsetX);
    const absY = Math.abs(offsetY);
    //console.log('startX:', startX, 'startY:', startY, 'offsetX:', offsetX, 'offsetY:', offsetY, 'EDGE_THRESHOLD:', EDGE_THRESHOLD);

    const isHorizontal = absX > absY;
    const xEdgeLeft = startX <= EDGE_THRESHOLD;
    const xEdgeRight = startX >= window.innerWidth - EDGE_THRESHOLD;
    const yEdgeTop = startY <= EDGE_THRESHOLD;
    const yEdgeBottom = startY >= window.innerHeight - EDGE_THRESHOLD;
    //console.log('isHorizontal:', isHorizontal, 'xEdgeLeft:', xEdgeLeft, 'xEdgeRight:', xEdgeRight, 'yEdgeTop:', yEdgeTop, 'yEdgeBottom:', yEdgeBottom);

    // Om horisontell swipe
    if (isHorizontal) {
      if (offsetX < 0) return xEdgeRight ? 'from-right' : 'left';
      if (offsetX > 0) return xEdgeLeft ? 'from-left' : 'right';
    }

    // Om vertikal swipe
    if (!isHorizontal) {
      if (offsetY < 0) return yEdgeBottom ? 'from-down' : 'up';
      if (offsetY > 0) return yEdgeTop ? 'from-up' : 'down';
    }

    return null;
  };

  const reachesThreshold = () => {
    const absX = Math.abs(position.offsetX);
    const absY = Math.abs(position.offsetY);
    const passedX = absX > position.SWIPE_THRESHOLD;
    const passedY = absY > position.SWIPE_THRESHOLD;
    const passedThreshold = passedX || passedY;
    return passedThreshold;
  };

  const triggerSwipe = async (direction: string) => {

    //console.log('TouchEvent:', direction, 'NamesLeft:', nameplate.names);
    if (direction.includes('from')) {
      await api.postReactions();
    }
    switch (direction) {
      case 'from-up':
        api.fetchReactions(Reaction.FAVORITE)
        state.display = Display.FAVORITES;
        break;
      case 'up':
        addReaction(nameplate.names[nameplate.currentIndex].nameInfoId, Reaction.FAVORITE);
        break;
      case 'from-down':
        state.display = Display.MENU;
        break;
      case 'down':
        break;
      case 'from-left':
        api.fetchReactions(Reaction.DISLIKE)
        state.display = Display.DISLIKED;
        break;
      case 'left':
        addReaction(nameplate.names[nameplate.currentIndex].nameInfoId, Reaction.DISLIKE);
        break;
      case 'from-right':
        api.fetchReactions(Reaction.LIKE)
        state.display = Display.LIKED;
        break;
      case 'right':
        addReaction(nameplate.names[nameplate.currentIndex].nameInfoId, Reaction.LIKE);
        break;
    }
    //console.log('reactions:', state.reactionsToAdd)

    if (!direction.includes('from')) {
      nameplate.names.shift();
      if (nameplate.names.length < 2) {
        nameQuery.pagenum++;
        console.log(nameQuery)
        api.postReactions();
        console.log('wtf', nameQuery.letters[nameQuery.currentIndex])
        const data = await api.getNamesByLetter(nameQuery.letters[nameQuery.currentIndex], nameQuery.pagenum);
        nameplate.names = [...data]
      }
      console.log(nameplate.names)
      if (nameplate.names.length > 1) {
        nameplate.nextIndex = nameplate.currentIndex + 1;
      } else {
        nameplate.nextIndex = 0;
      }
      if (nameplate.names.length === 0) {
        state.display = Display.CARD;
      }
      console.log('next:' ,nameplate.nextIndex, 'current:' ,nameplate.currentIndex, nameplate.names.length)
    }
  };
  const addReaction = (nameInfoId: number, reaction: Reaction, isAFavorite = false) => {
    if (isAFavorite) {
      reaction = 1;
    }
    state.reactionsToAdd.push({ nameInfoId, reaction, isAFavorite });
  };
  const setStartPos = (touch: Touch) => {
    position.startX = touch.clientX;
    position.startY = touch.clientY;
  };
  const setOffsetPos = (touch: Touch) => {
    position.offsetX = touch.clientX - position.startX;
    position.offsetY = touch.clientY - position.startY;
  };
  const setStylingParams = () => {
    nameplate.rotation = position.offsetX * 0.01;
    nameplate.scale = 1 - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 1000;
    nameplate.opacity = styleDeafult.opacity - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 800;
    //console.log('styling')
        const totalOffset = Math.abs(position.offsetX) + Math.abs(position.offsetY);
          nameplate.nextCardOpacity = Math.min(totalOffset, 200) / 200;
          //console.log('opacity value:', nameplate.nextCardOpacity)
  };
  const resetOffset = () => {
    position.offsetX = 0;
    position.offsetY = 0;
  };
  const resetStylingParams = () => {
    nameplate.rotation = 0;
    nameplate.opacity = styleDeafult.opacity;
    nameplate.scale = 1;
    nameplate.nextCardOpacity = 0;
    //console.log('opacity value RESET:', nameplate.nextCardOpacity)
  };
    const getGenderClass = (genderInt: number) => {
      const genderMap: {[key: number]: string} = {
        0: 'female',
        1: 'male',
      };
      return `gender-${genderMap[genderInt] || 'unknown'}`;
    };
  const nextCardStyle = computed(() => {
      return  { opacity: nameplate.nextCardOpacity }
       
    });

</script>

<template>
  <p>swipecard</p>
 <transition name="nameplate-fade">
    <div v-if="nameplate.names[nameplate.nextIndex]" class="nameplate"
        :class="getGenderClass(nameplate.names[nameplate.nextIndex]?.gender)"
        :key="'old-' + nameplate.nextIndex"
        :style="nextCardStyle">
        <h1>{{ nameplate.names[nameplate.nextIndex].name }}</h1>
        <p>{{ nameplate.names[nameplate.nextIndex].description }}</p>
        <p>{{ nameplate.names[nameplate.nextIndex].amount }}</p>
    </div>
</transition>
</template>

<style scoped>

</style>
