<template>
    <p>menu</p>
    <div id="letter-wheel" v-if="state.isLoggedIn && state.display === Display.MENU">
        <button @click="logout">Logga ut</button>
        <div class="menu">
            <img src="../../assets/svgs/settings-icon.svg" alt="Settings" width="100px"
                @click.prevent="toggleSubmenu(SubMenu.SETTINGS)" />
            <h1 class="gradiant-text" @click.prevent="toggleSubmenu(SubMenu.MAIN)">
                Name Space
            </h1>
            <img src="../../assets/svgs/add-partner.svg" alt="Add partner" width="100px"
                @click.prevent="toggleSubmenu(SubMenu.PARTNER)" />
        </div>
        <div v-if="state.submenu == SubMenu.MAIN">
            <div class="range-slider-wrapper">
                <p class="gradiant-text">Minsta antal med namnet</p>
                <input type="range" v-model="sliderValue" :min="0" :max="1000" :step="10" class="range-slider" />
                <p class="gradiant-text">{{ sliderValue }}</p>
            </div>


            <div id="letter-wheel-container" ref="scrollContainer" @scroll="onScroll">
                <div class="ghost-box"></div>
                <div class="box gradiant-text" v-for="(item, index) in nameQuery.letters" :key="index"
                    :style="getTransform()" @click="fetchData(index, nameQuery.pagenum, true, sliderValue)">

                    {{ item }}

                </div>

                <div class="ghost-box"></div>

            </div>
            <button @click="resetDisplay">Go Back</button>
        </div>
        <div v-if="state.submenu == SubMenu.SETTINGS">
            <p>settings</p>
        </div>
        <div v-if="state.submenu == SubMenu.PARTNER">
            <p>partner</p>
            <form class="partner-form register-form" @submit.prevent="sendPartnerRequest">
                <input type="text" name="partner" placeholder="Användarnamn/E-mail" required />
                <button type="submit">Skicka partnerförfrågan</button>

            </form>
        </div>
    </div>
</template>

<script setup lang="ts">
import { state, Display, nameQuery, SubMenu, nameplate } from '@scripts/state.ts'
import * as api from '@scripts/api.ts'
import { ref, reactive } from 'vue';
import axios from 'axios'
import { useRouter } from 'vue-router'
const router = useRouter();


const scrollContainer = ref<HTMLElement | null>(null);
const sliderValue = ref(50)
//imported from swipecard



//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
const fetchData = async (index: number, pagenum: number, isStartRequest = false, sliderValue: number) => {
    nameQuery.currentIndex = index;
    const letter = nameQuery.letters[nameQuery.currentIndex]
    console.log('index', index, nameQuery.currentIndex, 'letter', nameQuery.letters[nameQuery.currentIndex], letter)
    //letter = letter === '?' ? nameQuery.letters[Math.floor(Math.random() * 29) + 1] : letter;
    try {
        const response = await api.getNamesByLetter(letter, pagenum, sliderValue); // Byt ut med din API-URL
        if (isStartRequest) {
            nameplate.names = [...response]
        } else {

            nameplate.names.push(...response);
        }
        console.log("HERE: ", nameplate.names); // Sätt datan i din names array
        state.display = Display.CARD;
        router.push('/card'); 
    } catch (error) {
        console.error('API-anropet misslyckades:', error);
    }
};

const getTransform = () => {
    return {
        transition: 'transform 0.1s ease, font-size 0.1s ease',
    };
};
const resetDisplay = () => {
    state.display = Display.CARD;
    console.log('clicked button')
};

const toggleSubmenu = (target: typeof SubMenu[keyof typeof SubMenu]) => {
    if (state.submenu === target) {
        state.submenu = SubMenu.MAIN;
    }
    else {
        state.submenu = target;
    }
};

const logout = () => {
    axios.post('http://192.168.50.9:5228/api/account/logout', {}, { withCredentials: true })
        .then(() => {
            state.isLoggedIn = false;
            // window.location.href = '/login';  // eller använd vue-router för redirect
        }).catch((error) => {
            console.error("Logout failed:", error);
        });
};

const onScroll = () => {

    // Vänta 100ms innan scroll-beräkningen utförs

    const container = scrollContainer.value;
    if (!container) return;
    const boxes = container.querySelectorAll('.box');

    const containerRect = container.getBoundingClientRect();
    const containerCenterX = containerRect.left + containerRect.width / 2;

    boxes.forEach((box) => {
        const rect = box.getBoundingClientRect();
        const boxCenterX = rect.left + rect.width / 2;

        // Beräkna avståndet från boxens mittpunkt till containerns mittpunkt
        const distanceFromCenter = Math.abs(boxCenterX - containerCenterX);

        // Maximal tolerans, hur mycket boxen kan vara bort från mitten innan den är som minst
        const maxDistance = containerRect.width / 2;

        // Beräkna skalan och fontstorleken baserat på avståndet från mitten
        let scale = 1 - (distanceFromCenter / maxDistance) * 0.3; // Max scale minskas med 30% ju längre från mitten
        scale = Math.max(scale, 0.7);
        const fontSize = Math.max(1.5, 27 * scale); // Justera den här formeln beroende på hur stor font du vill
        let distanceFactor = distanceFromCenter / maxDistance;
        let opacity = 1 - (0.8 * distanceFactor);

        if (opacity < 0.2) opacity = 0.2;
        console.log(opacity)
        // Applicera den beräknade skalan och fontstorleken
        if (box instanceof HTMLElement) {

            box.style.transform = `scale(${scale})`;
            box.style.opacity = opacity.toString();;
            box.style.fontSize = `${fontSize}rem`;
        }
        /* box.style.background = `radial-gradient(
           ellipse at center,
           rgb(255, 255, 255) 0%,
           rgba(0, 0, 255, 0) 70%
         )`;*/
    });
};

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