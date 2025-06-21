<template>
    <img :src="getGenderIcon(gender)" alt="Gender Icon" width="50px" />
    <h3>{{ name }}</h3>
    <p>{{ count }}</p>
    <img v-if="partnerReaction != null" :src="getReactionSvg(partnerReaction)" alt="Partner reaction"
        width="50px" />
</template>

<script setup lang="ts">
import { reactive } from 'vue';

defineProps<{
    name: string;
    count: number;
    gender: number; //should change to enum 
    partnerReaction: number | null//should also be enum
}>();
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
 const getGenderIcon = (genderInt: number) => {
      if (genderInt === 0){
        return svgFileLocations.gender.female;
      }
      else if (genderInt === 1){
        return svgFileLocations.gender.male;
      }
      else {
        return svgFileLocations.gender.unknown;
      }
    };
    const getReactionSvg = (reaction: number) => {
      switch (reaction){
        case 2:
          return svgFileLocations.reaction.favorite;
        case 1: 
          return svgFileLocations.reaction.liked;
        case 0: 
          return svgFileLocations.reaction.disliked;
        default:
          return undefined;
      }
    }
</script>

<style scoped></style>