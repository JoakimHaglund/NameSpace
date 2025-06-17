<template>
  <p>partner</p>
            <form class="partner-form register-form" @submit.prevent="sendPartnerRequest">
                <input type="text" name="partner" placeholder="Användarnamn/E-mail" required />
                <button type="submit">Skicka partnerförfrågan</button>

            </form>
</template>

<script setup lang="ts">
import { state } from '@scripts/state.ts'
import * as api from '@scripts/api.ts'
import { ref } from 'vue';




const scrollContainer = ref<HTMLElement | null>(null);
const sliderValue = ref(50)
//imported from swipecard
const sendPartnerRequest = async (event: Event) => {

    const form = event.target as HTMLFormElement;
    const partner = (form.elements.namedItem('partner') as HTMLInputElement).value;
    if (partner !== null) {
        const response = await api.addPartnerRequest(partner);
        console.log("Login: ", response);
        if (response.status === 200) {
            state.showRegisterForm = false;

        } else {
            state.errorMessage = response;
        }
    } else {
        state.errorMessage = 'no partner from submit';
    }
};
</script>

<style scoped>

</style>