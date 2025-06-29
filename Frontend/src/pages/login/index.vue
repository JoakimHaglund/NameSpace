<template>
    <div v-if="!state.isLoggedIn && !state.showRegisterForm">
        <h1 class="gradiant-text">NameSpace</h1>
        <form class="login-form" @submit.prevent="login">
            <input type="text" v-model="email" placeholder="Användarnamn" required />
            <input type="password" v-model="password" placeholder="Lösenord" required />
            <button type="submit">Logga in</button>
            <button type="button" @click.prevent="toggleRegisterForm">Registera</button>
        </form>
        <p class="gradiant-text" v-if="state.errorMessage">{{ state.errorMessage }}</p>
    </div>
    <div v-if="!state.isLoggedIn && state.showRegisterForm">
        <h1 class="gradiant-text">NameSpace</h1>
        <form class="register-form" @submit.prevent="register">
            <input type="text" v-model="username" placeholder="Användarnamn" required />
            <input type="email" v-model="registerEmail" placeholder="E-mail" required />
            <input type="password" v-model="registerPassword" placeholder="Lösenord" required />
            <input type="password" v-model="confirmPassword" placeholder="Lösenord" required />
            <div id="register-form-buttons-wrapper">
                <button type="button" @click.prevent="toggleRegisterForm">Gå tillbaka</button>
                <button type="submit">Registera</button>
            </div>
        </form>
        <p class="gradiant-text" v-if="state.errorMessage">{{ state.errorMessage }}</p>
    </div>
</template>

<script setup lang="ts">
import * as api from '@scripts/api.ts';
import { state } from '@scripts/state.ts';
import { ref } from 'vue';
import { useRouter } from 'vue-router'
const router = useRouter();
const email = ref('');
const password = ref('');
const registerEmail = ref('');
const registerPassword = ref('');
const username = ref('');
const confirmPassword = ref('');
const toggleRegisterForm = () => {
    state.showRegisterForm = !state.showRegisterForm;
};
const login = async () => {

    const response = await api.loginUser(email.value, password.value);
    console.log("Login: ", response);

    if (response.status === 200) {
        state.isLoggedIn = true;
        //fetchData(nameQuery.letter, nameQuery.pagenum);
        router.push('/menu'); 
    } else {
        state.errorMessage = response;
    }
};
const register = async () => {
    if (password.value === confirmPassword.value) {
        const response = await api.registerUser(username.value, registerEmail.value, registerPassword.value);
        console.log("Login: ", response);
        if (response.status === 200) {
            state.showRegisterForm = false;

        } else {
            state.errorMessage = response;
        }
    } else {
        state.errorMessage = 'Passwords do not match'
    }
};
</script>

<style scoped></style>