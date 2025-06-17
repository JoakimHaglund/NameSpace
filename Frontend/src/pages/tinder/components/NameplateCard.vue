<template>
    <transition name="nameplate-fade">
        <div v-if="nameInfo" class="nameplate"
            :class="getGenderClass(nameInfo.gender)" :key="key"
            :style="styleObject">
            <h1>{{ nameInfo.name }}</h1>
            <p>{{ nameInfo.descriptionOfName }}</p>
            <p>{{ nameInfo.count }}</p>
        </div>
    </transition>

    <!-- Nuvarande kortet, fades in 
    <transition name="nameplate-fade">
        <div v-if="nameplate.names[nameplate.currentIndex]" class="nameplate nameplate-current"
            :class="getGenderClass(nameplate.names[nameplate.currentIndex]?.gender)"
            :key="'current-' + nameplate.currentIndex" @touchstart="handleSwipe" @touchmove="handleSwipe"
            @touchend="handleSwipe" :style="shouldApplyOffsets ? {
                transform: 'translate(' + position.offsetX + 'px, ' + position.offsetY + 'px)' +
                    ' rotate(' + nameplate.rotation + 'deg)' +
                    ' scale(' + nameplate.scale + ')',
                opacity: nameplate.opacity
            } : null">
            <h1>{{ nameplate.names[nameplate.currentIndex].name }}</h1>
            <p>{{ nameplate.names[nameplate.currentIndex].descriptionOfName }}</p>
            <p>{{ nameplate.names[nameplate.currentIndex].count }}</p>
            <p v-if="nameplate.names[nameplate.currentIndex].partnerReaction">
                {{ nameplate.names[nameplate.currentIndex].partnerReaction }}</p>
        </div>
    </transition>-->
</template>

<script setup lang="ts">
import type  {NameInfo} from '../../../scripts/state.ts';
defineProps<{
    nameInfo: NameInfo
    key: string
    styleObject?: Record<string, any> | null
}>();
    const getGenderClass = (genderInt: number) => {
      const genderMap: Record<number, string> = {
        0: 'female',
        1: 'male',
      };
      return `gender-${genderMap[genderInt] || 'unknown'}`;
    };
</script>
<style scoped></style>