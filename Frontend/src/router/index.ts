import { createRouter, createWebHistory } from 'vue-router'
import { checkLoginStatus } from '../scripts/api.ts';
import {state} from '../scripts/state.ts';
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
   /* {
      path: '/',
      name: 'home',
      component: HomeView,
    },
  /*  {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },*/
     {
      path: '/swipecard',
      name: 'swipecard',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../pages/tinder/index.vue'),
    },
     {
      path: '/login',
      name: 'login',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../pages/login/index.vue'),
    },
    {
      path: '/confirm-email',
      name: 'confirm-email',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../components/completeRegistration.vue'),
    },
    {
     path: '/menu',
      name: 'menu',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
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
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../pages/tinder/index.vue'),
    },
    {
     path: '/list/:list',
      name: 'list',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
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
