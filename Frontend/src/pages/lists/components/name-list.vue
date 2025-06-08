<template>
    <div class="list-wrapper">
        <div class="list-item" v-for="(item, index) in list" :key="index" :class="getGenderClass(item?.gender)"
            @touchstart="horizontalSwipe($event, index, list)" @touchmove="horizontalSwipe($event, index, list)"
            @touchend="horizontalSwipe($event, index, list)">

            <NameListItem :name="item.name" :count="item.count" :gender="item.gender"
                :partnerReaction="item.partnerReaction"></NameListItem>
        </div>

    </div>
</template>

<script setup lang="ts">
import NameListItem from './name-list-item.vue';
import { reactive } from 'vue'
type NameObject = {
    name: string;
    count: number;
    gender: number; //should change to enum 
    partnerReaction: number //should also be enum
}
defineProps<{
    list: NameObject[]
}>();
const swipeItem = reactive({
    currentElement: null as HTMLElement | null,
    offsetX: 0,
    initalLeft: 0
});
//needs major rework, it's a mess
const horizontalSwipe = (event: TouchEvent, index: number, items: NameObject[]) => {

    event.preventDefault();
    console.log('touch event:', event.type);
    swipeItem.currentElement = event.target as HTMLElement;
    const rect = swipeItem.currentElement.getBoundingClientRect();
    if (event.type === "touchstart") { //TouchEventStarts
        //get the element

        swipeItem.offsetX = event.touches[0].clientX - rect.left;

        swipeItem.initalLeft = rect.left;

    } else if (event.type === "touchmove") { //Client moves finger
        if (!swipeItem.currentElement) return;

        const newX = event.touches[0].clientX - swipeItem.offsetX;
        console.log(rect.top)
        // Flytta elementet direkt
        swipeItem.currentElement.style.position = 'absolute'
        swipeItem.currentElement.style.left = newX + 'px'
        // swipeItem.currentElement.style.left = rect.top + 'px';
    } else if (event.type === "touchend") { //Client lifts finger
        if (!swipeItem.currentElement) return;
        const x = parseInt(swipeItem.currentElement.style.top || '0')

        // Om vi släpper under en viss y-position, ta bort elementet
        if (x > 400) {
            items.splice(index, 1)
            swipeItem.currentElement = null
        } else {
            // Annars, återställ till originalposition
            swipeItem.currentElement.style.transition = '0.3s'
            // swipeItem.currentElement.style.left = swipeItem.initialLeft + 'px'
            // swipeItem.currentElement.style.top = swipeItem.initialTop + 'px'
        }
    }
};
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
</script>

<style scoped></style>