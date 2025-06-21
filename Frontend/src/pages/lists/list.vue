<template>
    <div id="" v-if="state.isLoggedIn">
        <h1 class="gradiant-text" @click="resetDisplay">{{listToGet}}</h1>
        <div v-if="list.length === 0 && !hasFetched">
            <div class="spinner"></div>
        </div>
        <div v-if="list.length === 0 && hasFetched" @click="resetDisplay">
            <p class="gradiant-text">Inga {{hasFetched}} hittades :(</p>
        </div>

        <!-- När login-statusen är kontrollerad -->
        <div v-else>
            <NameListItem :list="list"></NameListItem>    
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

const route = useRoute();
const router = useRouter();

const listToGet = route.params.list;
const list = ref<NameInfo[]>([]);
const hasFetched = ref(false);

onMounted(async () => {
  try {
    if(listToGet === "favorites"){
        await api.fetchReactions(2)
        list.value = lists.favorites
        hasFetched.value = lists.hasFetched.favorites
    }
    else if (listToGet === "disliked"){
        await api.fetchReactions(0)
        list.value = lists.disliked
        hasFetched.value = lists.hasFetched.disliked
    }
    else if (listToGet === "liked"){
        await api.fetchReactions(1)
        list.value = lists.liked
        hasFetched.value = lists.hasFetched.liked
    }
  } catch (e) {
    console.error('Fuck, kunde inte hämta listan', e)
  }
})

const resetDisplay = () => {
    //rout to card
    console.log('clicked button')
    router.push('/card'); 
};

</script>

<style scoped></style>