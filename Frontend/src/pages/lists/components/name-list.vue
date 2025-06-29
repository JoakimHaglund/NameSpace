<template>
  <div class="list-wrapper">
    <div class="card-container" v-for="item in list" :key="item.nameInfoId">
      <div class="list-item background-item">
        <div id="move">
          <p>move</p>
        </div>
        <div id="delete">
          <p>Delete</p>
        </div>
      </div>

      <div class="list-item middle-card" :style="styleMap.get(item.nameInfoId)"></div>

      <NameListItem class="list-item display-item" 
        :class="getGenderClass(item?.gender)"
        :style="styleMap.get(item.nameInfoId)" 
        :name="item.name" :count="item.count" 
        :gender="item.gender"
        :partnerReaction="item.partnerReaction" 
        @touchstart.prevent="swipeMap.get(item.nameInfoId)?.handleSwipe($event)"
        @touchmove.prevent="swipeMap.get(item.nameInfoId)?.handleSwipe($event)"
        @touchend.prevent="swipeMap.get(item.nameInfoId)?.handleSwipe($event)" />
    </div>
  </div>
</template>

<script setup lang="ts">
import NameListItem from './name-list-item.vue';
import { reactive, watch } from 'vue'
import { useSwipe } from '@scripts/useSwipe';
import type { NameInfo } from '@scripts/state.ts';

const emit = defineEmits(['remove']);
const props = defineProps<{
  list: NameInfo[]
}>();

const swipeMap = reactive(new Map<number, ReturnType<typeof useSwipe>>())
const styleMap = reactive(new Map<number, Record<string, string> | null>())

enum Gender {
  Female = 0,
  Male = 1,
  Unknown = 2
}

const getGenderClass = (genderInt: number) => {
  switch (genderInt) {
    case Gender.Female:
      return 'gender-female';
    case Gender.Male:
      return 'gender-male';
    default:
      return 'gender-unknown';
  }
};

watch(
  () => props.list,
  newList => {
    for (const id of swipeMap.keys()) {
      if (!newList.find(i => i.nameInfoId === id)) swipeMap.delete(id)
    }
    newList.forEach(item => {
      const half = window.innerWidth / 4
      let lastPos = { offsetX: 0 }
      if (!swipeMap.has(item.nameInfoId)) {
        swipeMap.set(item.nameInfoId, useSwipe({
          beforeStart() {
            styleMap.set(item.nameInfoId, null)
          },
          onUpdate(pos) {
            lastPos = pos;
            const half = window.innerWidth / 3
            const x = Math.min(Math.abs(pos.offsetX), half) *
              (pos.offsetX < 0 ? -1 : 1)
            styleMap.set(item.nameInfoId, { transform: `translate(${x}px)` })
          },
          onEnd(dir) {

            const reached = Math.abs(lastPos.offsetX) > half
            console.log(half, reached, styleMap)
            if (reached) emit('remove', item.nameInfoId, dir)
          }
        }))
      }
    })
  },
  { immediate: true, deep: true }
)
</script>

<style scoped>
.list-wrapper {
  display: grid;
  justify-content: center;
  align-content: center;
  width: 100vw;
  text-align: center;
  word-break: break-word;
  overflow-wrap: break-word;
  white-space: normal;
}

.card-container {
  position: relative;
  /* Viktigast av allt */
  margin: 0.5rem;
  width: 90dvw;
  display: flex;
  justify-content: center;
}

.list-item {
  display: grid;
  grid-auto-flow: column;
  width: 100%;

  border-radius: 10px;
  box-shadow: 5px 5px 10px rgba(0, 0, 0, 0.2);
  box-shadow: inset 5px 5px 15px rgba(0, 0, 0, 0.2);
}

.display-item {
  padding: 2rem;
  position: relative;
  z-index: 2;
  grid-template-columns: 0.5fr 3fr 1fr 1fr;
  gap: 10px;
  align-items: center;
}

.middle-card {
  position: absolute;
  inset: 0;
  background-color: #ddd;
  /* typ en soft grå som sticker ut */
  z-index: 1;
  border-radius: 10px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
}

.background-item {
  position: absolute;
  overflow: hidden;
  inset: 0;
  z-index: 0;
  background-color: #ff00dd;
  display: flex;
}

#delete,
#move {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  font-weight: bold;
  padding: 50px;
}

p {
  font-size: 4rem;
}

#move {
  background-color: rgb(51, 255, 0);
  justify-content: start;

}

#delete {
  background-color: red;
  justify-content: end;
}

.list-item h3 {
  font-size: 3rem;
  justify-self: left;
}
</style>