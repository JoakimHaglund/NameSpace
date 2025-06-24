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
import {ref, onMounted, reactive} from 'vue';
import type {NameInfo} from '@scripts/state.ts';
import * as api from '@scripts/api.ts'
import type { SwipeDirection } from '@/scripts/useSwipe';
import { Reaction, parseReactionType, stringifyReactionType  } from '@scripts/reactionType'
const route = useRoute()
const router = useRouter()


const list = ref<NameInfo[]>([])
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

const resetDisplay = () => {
    //rout to card
    console.log('clicked button')
    router.push('/card'); 
};
function handleRemove(id: number, dir: SwipeDirection) {
    console.log(reaction)
  list.value = list.value.filter(n => n.nameInfoId !== id)
}

</script>

<style scoped></style>