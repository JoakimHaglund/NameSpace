<template>
    <div id="" v-if="state.isLoggedIn && state.display === Display.FAVORITES">
        <h1 class="gradiant-text" @click="resetDisplay">Favoriter</h1>
        <div v-if="lists.favorites.length === 0 && !lists.hasFetched.favorites">
            <div class="spinner"></div>
        </div>
        <div @click="resetDisplay" v-if="lists.favorites.length === 0 && lists.hasFetched.favorites">
            <p class="gradiant-text">Inga favoriter hittades :(</p>
        </div>

        <!-- När login-statusen är kontrollerad -->
        <div v-else>
            <div class="list-wrapper">
                <div class="list-item" v-for="(item, index) in lists.favorites" :key="index"
                    :class="getGenderClass(item?.gender)">
                    <img :src="getGenderIcon(item?.gender)" alt="Gender Icon" width="50px" />
                    <h3>{{ item.name }}</h3>
                    <p>{{ item.antal }}</p>
                    <img v-if="item.partnerReaction != null" :src="getReactionSvg(item.partnerReaction)"
                        alt="Partner reaction" width="50px" />
                </div>
            </div>
        </div>
    </div>
    <div id="" v-if="state.isLoggedIn && state.display === Display.DISLIKED">
        <h1 class="gradiant-text" @click="resetDisplay">Ogillade</h1>
        <div v-if="lists.disliked.length === 0 && !lists.hasFetched.disliked">
            <div class="spinner"></div>
        </div>
        <div v-if="lists.disliked.length === 0 && lists.hasFetched.disliked" @click="resetDisplay">
            <p class="gradiant-text">Inga disliked hittades :(</p>
        </div>

        <!-- När login-statusen är kontrollerad -->
        <div v-else>
            <div class="list-wrapper">
                <div class="list-item" v-for="(item, index) in lists.disliked" :key="index"
                    :class="getGenderClass(item?.gender)" @touchstart="horizontalSwipe($event, index, lists.disliked)"
                    @touchmove="horizontalSwipe($event, index, lists.disliked)"
                    @touchend="horizontalSwipe($event, index, lists.disliked)">
                    <img :src="getGenderIcon(item?.gender)" alt="Gender Icon" width="50px" />
                    <h3>{{ item.name }}</h3>
                    <p>{{ item.antal }}</p>
                    <img v-if="item.partnerReaction != null" :src="getReactionSvg(item.partnerReaction)"
                        alt="Partner reaction" width="50px" />
                </div>

            </div>
        </div>

    </div>
    <div id="" v-if="state.isLoggedIn && state.display === Display.LIKED">
        <h1 class="gradiant-text" @click="resetDisplay">Gillade</h1>
        <div v-if="lists.liked.length === 0 && !lists.hasFetched.liked">
            <div class="spinner"></div>
        </div>
        <div v-if="lists.liked.length === 0 && lists.hasFetched.liked" @click="resetDisplay">
            <p class="gradiant-text">Inga liked hittades :(</p>
        </div>

        <!-- När login-statusen är kontrollerad -->
        <div v-else>
            <div class="list-wrapper">
                <div class="list-item" v-for="(item, index) in lists.liked" :key="index"
                    :class="getGenderClass(item?.gender)">
                    <img :src="getGenderIcon(item?.gender)" alt="Gender Icon" width="50px" />
                    <h3>{{ item.name }}</h3>
                    <p>{{ item.antal }}</p>
                    <img v-if="item.partnerReaction != null" :src="getReactionSvg(item.partnerReaction)"
                        alt="Partner reaction" width="50px" />
                </div>
            </div>
        </div>
    </div>


</template>

<script lang="ts">

</script>

<style scoped></style>