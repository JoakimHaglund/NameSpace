<template>
 <div class="range-slider-wrapper">
                <p class="gradiant-text">Minsta antal med namnet</p>
                <input type="range" v-model="sliderValue" :min="0" :max="1000" :step="10" class="range-slider" />
                <p class="gradiant-text">{{ sliderValue }}</p>
            </div>


            <div id="letter-wheel-container" ref="scrollContainer" @scroll="onScroll">
                <div class="ghost-box"></div>
                <div class="box gradiant-text" v-for="(item, index) in nameQuery.letters" :key="index"
                    :style="getTransform()" @click="goToCardPage(index, sliderValue)">

                    {{ item }}

                </div>

                <div class="ghost-box"></div>

            </div>
</template>

<script setup lang="ts">
import { state, Display, nameQuery,  nameplate } from '@scripts/state.ts'
import { getNamesByLetter } from '@scripts/api.ts'
import { ref } from 'vue';
import { useRouter } from 'vue-router'

const router = useRouter();
const scrollContainer = ref<HTMLElement | null>(null);
const sliderValue = ref(50)

const goToCardPage = async (index: number,  sliderValue: number) => {
    nameQuery.currentIndex = index;
    nameQuery.minCount = sliderValue

    //move to card page
    router.push('/card'); 

};

const getTransform = () => {
    return {
        transition: 'transform 0.1s ease, font-size 0.1s ease',
    };
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
</script>

<style scoped>

</style>