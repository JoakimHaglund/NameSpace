<template>
    <div id="letter-wheel" v-if="state.isLoggedIn">
        <button @click="logout">Logga ut</button>
        <Nav></Nav>
        <RouterView />
        <div v-if="state.submenu == SubMenu.MAIN">
            <button @click="resetDisplay">Go Back</button>
        </div>
    </div>
</template>

<script setup lang="ts">
import Nav from './components/nav.vue'
import { state, Display, SubMenu } from '@scripts/state.ts'

import { logout } from '@scripts/api.ts'

const resetDisplay = () => {
    console.log('clicked button')
};

</script>

<style scoped></style>