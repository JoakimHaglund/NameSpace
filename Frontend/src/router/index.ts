import { createRouter, createWebHistory } from 'vue-router'
import { checkLoginStatus } from '../scripts/api.ts';
import { state } from '../scripts/state.ts';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/swipecard',
      name: 'swipecard',
      component: () => import('../pages/tinder/index.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../pages/login/index.vue'),
    },
    {
      path: '/confirm-email',
      name: 'confirm-email',
      component: () => import('../components/completeRegistration.vue'),
    },
    {
      path: '/menu',
      name: 'menu',
      component: () => import('../pages/main-menu/index.vue'),
      children: [
        {
          path: '',
          component: () => import('../pages/main-menu/components/letter-wheel.vue')
        },
        {
          path: 'settings',
          component: () => import('../pages/main-menu/components/settings.vue')
        },
        {
          path: 'partner',
          component: () => import('../pages/main-menu/components/partner-form.vue')
        },
      ]
    },
    {
      path: '/card',
      name: 'card',
      component: () => import('../pages/tinder/index.vue'),
    },
    {
      path: '/list/:list',
      name: 'list',
      component: () => import('../pages/lists/list.vue'),
      props: true

    },
  ],
})
router.beforeEach(async (to, from, next) => {
  if (to.path === '/confirm-email') {
    return next()
  }

  if (!state.isCheckingLogin) {
    state.isCheckingLogin = true
    try {
      await checkLoginStatus()
    } catch (err) {
      state.isLoggedIn = false
    } finally {
      state.isCheckingLogin = false
    }
  }

  if (state.isLoggedIn) {
    return next()
  }
  if (to.path !== '/login') {
    return next('/login')
  }

  next()
})
export default router
