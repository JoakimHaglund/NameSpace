<template>
    <div class="spinner-wrapper">
      <div class="spinner"></div>
    </div>
</template>

<style scoped>
.spinner-wrapper {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh; /* hela skärmhöjden */
    width: 100vw;  /* hela bredden */
}

.spinner {
    border: 2dvw solid #f3f3f3;
    /* light gray */
    border-top: 2dvw solid #3498db;
    /* blå */
    border-radius: 50%;
    width: 20dvw;
    height: 20dvw;
    animation: spin 2s linear infinite;
    margin: 50px auto;
}
</style>