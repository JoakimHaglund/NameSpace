<template>
    <h1 class="gradiant-text">NameSpace</h1>
    <p class="gradiant-text register-message">{{ status }}</p>
    <p v-if="status" class="gradiant-text register-message">Du skickas vidare automatiskt om {{ seconds }}</p>
</template>

<script setup lang="ts">
import * as api from '@scripts/api.ts';
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const status = ref('');
const token = route.query.token;
const email = route.query.email;
const seconds = ref(5);

onMounted(async () => {
    try {
        if (token && email) {
            console.log('Token finns:', token);
            status.value = await api.completeRegistration(token.toString(), email.toString());
            console.log('status:', status);
        }
        else {
            console.log('Token finns inte');
            status.value = 'Token finns inte';
        }
    }
    catch (error) {
        console.log('API-anrop misslyckades:', error);
        status.value = 'Verifieringen misslyckades, försök igen senare';
    }
    finally {
        const timer = setInterval(() => {
            seconds.value--;
            if (seconds.value === 0) {
                clearInterval(timer);
                router.push('/login');
            }
        }, 1000)
    }

});
</script>
<style scoped>
.register-message {
    text-align: center;
    font-weight: bolder;
    font-size: 3rem;
}
</style>
