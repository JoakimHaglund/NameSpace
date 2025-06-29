<template>
  <div>
    <img :src="getGenderIcon(gender)" alt="Gender Icon" width="50px" />
    <h3>{{ name }}</h3>
    <p>{{ count }}</p>
    <img :src="getReactionSvg(partnerReaction)" alt="Partner reaction"
        width="50px" />
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
// Importing
import femaleSvg from '@svgs/genders/female.svg'
import maleSvg from '@svgs/genders/male.svg'
import unknownGenderSvg from '@svgs/genders/unknown.svg'

import favoriteSvg from '@svgs/reactions/favorite.svg'
import likedSvg from '@svgs/reactions/liked.svg'
import dislikedSvg from '@svgs/reactions/disliked.svg'
import noReactionSvg from '@svgs/reactions/no-reaction.svg'
defineProps<{
    name: string;
    count: number;
    gender: number; //should change to enum 
    partnerReaction: number | null //should also be enum
}>();

 const getGenderIcon = (genderInt: number) => {
      if (genderInt === 0){
        return femaleSvg;
      }
      else if (genderInt === 1){
        return maleSvg;
      }
      else {
        return unknownGenderSvg;
      }
    };
    const getReactionSvg = (reaction: number | null) => {
      switch (reaction){
        case 2:
          return favoriteSvg;
        case 1: 
          return likedSvg;
        case 0: 
          return dislikedSvg;
        default:
          return noReactionSvg;
      }
    }
</script>

<style scoped>

h3 {
	font-size: 3rem;
	
}
</style>