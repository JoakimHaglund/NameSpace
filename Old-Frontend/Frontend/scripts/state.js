const { reactive } = Vue;

export const Display = Object.freeze({
  MENU: 'main-menu',
  CARD: 'swipecard',
  FAVORITES: 'favorites',
  LIKED: 'liked',
  DISLIKED: 'disliked'
});
export const SubMenu = Object.freeze({
  MAIN: 'main',
  SETTINGS: 'settings',
  PARTNER: 'add-partner',
});
export const Reaction = Object.freeze({
FAVORITE: 2,
LIKE: 1,
DISLIKE: 0
});
export const nameQuery = reactive({
  letters: [ 
    '?', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
    'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S',
    'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Å', 'Ä', 'Ö'
  ],
  currentIndex: 0,
  pagenum: 1,
  minCount: null,
  MaxCount: null,
});
export const state = reactive({
    isCheckingLogin: true,
    isLoggedIn: false,
    showRegisterForm: false,
    showNameplate: false,
    showSelector: true,
    showFavorites: false,
    display: Display.MENU,
    submenu: SubMenu.MAIN,
    reactionsToAdd: [],
  });
export const lists = reactive({
    favorites: [],
    liked: [],
    disliked: [],
    hasFetched: {
      favorites: false,
      liked: false,
      disliked: false,
    }
  });
