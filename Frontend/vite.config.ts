import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      '@scripts': fileURLToPath(new URL('./src/scripts', import.meta.url)),
      '@svgs': fileURLToPath(new URL('./src/assets/svgs', import.meta.url))
    },
  },
   server: {
    host: '192.168.50.9', // Gör så du kan nå den från andra enheter i nätverket
    port: 5500,       // Byt till vad fan du vill, bara du inte clashar med nåt
  }
})
