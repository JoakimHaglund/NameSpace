<template>
    <div class="menu">
        <router-link to="/menu/settings">
            <img src="../../../assets/svgs/settings-icon.svg" alt="Settings" width="100px" />
        </router-link>
        <router-link to="/menu" class="no-style">
            <h1 class="gradiant-text">Name Space</h1>
        </router-link>
        <router-link to="/menu/partner">
            <img src="../../../assets/svgs/add-partner.svg" alt="Add partner" width="100px"/>
        </router-link>
    </div>
</template>

<script setup lang="ts">

</script>

<style scoped>
.no-style {
  all: unset; /* tar bort allt */
  cursor: pointer; /* om du vill behålla klickbarheten */
  color: inherit; /* tar bort blå färg */
  text-decoration: none; /* tar bort understrykning */
}
</style>