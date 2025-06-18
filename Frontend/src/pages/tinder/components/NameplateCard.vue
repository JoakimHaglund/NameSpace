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
<style scoped>
.nameplate {
	position: absolute;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	padding: 2rem;
	display: flex;
	flex-direction: column;
	justify-content: center;
	align-items: center;
	box-shadow: 0 0 30px rgba(0, 0, 0, 0.1);
	border-radius: 1rem;
	text-align: center;
	word-break: break-word;
	overflow-wrap: break-word;
	white-space: normal;
}

.nameplate-fade-enter-active,
.nameplate-fade-leave-active {
	transition: opacity 0.4s ease, transform 0.4s ease;
	width: 80vw;
	height: 80vh;
}

.nameplate h3,
.nameplate p {
	max-width: 100%;
	word-break: break-word;
	overflow-wrap: break-word;
	white-space: normal;
}

.nameplate-fade-enter-from {
	opacity: 0;
	transform: scale(0.9);
}

.nameplate-fade-enter-to {
	opacity: 1;
	transform: scale(1);
}

.nameplate-fade-leave-from {
	opacity: 1;
	transform: scale(1);
}

.nameplate-fade-leave-to {
	opacity: 0;
	transform: scale(1.1);
}
</style>