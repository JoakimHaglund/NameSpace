<template>
    <div id="" v-if="state.isLoggedIn">
        <h1 class="gradiant-text" @click="resetDisplay">{{title}}</h1>
        <div v-if="list.length === 0 && !hasFetched">
            <div class="spinner"></div>
        </div>
        <div v-if="list.length === 0 && hasFetched" @click="resetDisplay">
            <p class="gradiant-text">Inga {{hasFetched}} hittades :(</p>
        </div>

        <!-- När login-statusen är kontrollerad -->
        <div v-else>
            <NameListItem :list="list" @remove="handleRemove"></NameListItem>    
        </div>
    </div>

</template>

<script setup lang="ts">
import {state, lists} from '@scripts/state.ts';
import NameListItem from './components/name-list.vue';
import { useRoute, useRouter } from 'vue-router';
import {ref, onMounted, reactive, onBeforeUnmount} from 'vue';
import type {NameInfo} from '@scripts/state.ts';
import * as api from '@scripts/api.ts'
import { SwipeDirection } from '@/scripts/useSwipe';
import { Reaction, parseReactionType, stringifyReactionType  } from '@scripts/reactionType'
const route = useRoute()
const router = useRouter()


const list = ref<NameInfo[]>([])
const itemsToRemove = ref<NameInfo[]>([])
const itemsToChange = ref<NameInfo[]>([])
const hasFetched = ref(false);
let title = '';
let reaction: Reaction
onMounted(async () => {
  try {
    reaction = parseReactionType(route.params.list);
    title = stringifyReactionType(reaction);
    list.value = await api.fetchReactions(reaction);
    hasFetched.value = true

    console.log('Listan hämtad:', list.value, route.params);
  } catch (e) {
    console.error('kunde inte hämta listan', e)
  }
})
onBeforeUnmount(async () => {
  try {
    await api.deleteReactions(itemsToRemove.value)
    await api.patchReactions(itemsToChange.value)
 
  } catch (e) {
    console.error('kunde inte hämta listan', e)
  }
})

const resetDisplay = () => {
    //rout to card
    console.log('clicked button')
    router.push('/card'); 
};
async function handleRemove(id: number, dir: SwipeDirection) {
  const reactionToChange = list.value.find(n => n.nameInfoId === id);
  if(reactionToChange === undefined) return;

  if (dir === SwipeDirection.LEFT){
    itemsToRemove.value.push(reactionToChange)
  }
  else if (dir === SwipeDirection.RIGHT){
    itemsToChange.value.push(reactionToChange)
  }
  else {
    return; //Something that shouldn't happen happend for now lets just return.
  }
  console.log('itemsToChange',itemsToChange.value, 'itemsToRemove',itemsToRemove.value)
  if (itemsToRemove.value.length >= 10){
    await api.deleteReactions(itemsToRemove.value)
  }
  if (itemsToChange.value.length >= 10){
    await api.patchReactions(itemsToChange.value)
  }

  list.value = list.value.filter(n => n.nameInfoId !== id)
}

</script>

<style scoped></style>