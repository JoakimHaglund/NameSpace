
<template>
  <RouterView v-if="!state.isCheckingLogin" />
</template>

<script setup lang="ts">
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { onMounted } from 'vue'
import * as api from './scripts/api.ts';
import {state} from './scripts/state.ts'
const router = useRouter()

onMounted(async () => {
  try {
    const response = await api.checkLoginStatus()
    if (state.isLoggedIn) {
      router.push('/menu')
    } else {
      router.push('/login')
    }
  } catch (err) {
    console.error("Kunde inte kolla login:", err)
    state.isLoggedIn = false
    router.push('/login')
  } finally {
    state.isCheckingLogin = false
  }
})

</script>



<style scoped>

</style>
