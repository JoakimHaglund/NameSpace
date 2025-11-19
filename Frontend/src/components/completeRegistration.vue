<template>
    <h1 class="gradiant-text">NameSpace</h1>
    <p class="gradiant-text">reg {{status}}</p>
</template>

<script setup lang="ts">
import * as api from '@scripts/api.ts';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';

console.log('reg laddas');
const status = ref('nostatus')

    onMounted(async () => { //behöver hämta min data någon ananstans 
        // Hämta query-param direkt från URL:n
        const route = useRoute()
        const token = route.query.token  // Hämtar 'token' från URL:en
        const email = route.query.email  // Hämtar 'email' från URL:en

        if (token && email){
            console.log('Token finns:', token);
            status.value = await api.completeRegistration(token.toString(), email.toString());
            console.log('status:', status);
        }
        else{
            console.log('Token finns inte');
        }

    });
</script>
<style scoped></style>
