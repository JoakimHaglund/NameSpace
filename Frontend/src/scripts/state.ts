import { reactive } from 'vue';

export enum Display {
  MENU,
  CARD,
  FAVORITES,
  LIKED,
  DISLIKED
};

type SubMenuType = typeof SubMenu[keyof typeof SubMenu];
export const SubMenu = Object.freeze({
  MAIN: 'main',
  SETTINGS: 'settings',
  PARTNER: 'add-partner',
});
export enum Reaction {
  FAVORITE = 2,
  LIKE = 1,
  DISLIKE = 0
};
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
  submenu: SubMenu.MAIN as SubMenuType,
  reactionsToAdd: [] as { nameInfoId: number, reaction: Reaction }[],
  errorMessage: null as string | null,
});

export type NameInfo = {
  nameInfoId: number,
  name: string,
  count: number
  descriptionOfName: string,
  partnerReaction: number | null
  gender: number
}
export const lists = reactive({
  favorites: [] as NameInfo[],
  liked: [] as NameInfo[],
  disliked: [] as NameInfo[],
  hasFetched: {
    favorites: false,
    liked: false,
    disliked: false,
  }
});
// move to card/tinder
export const nameplate = reactive({
  names: [] as NameInfo[],
  currentIndex: 0,
  nextIndex: 0,
  rotation: 0,
  scale: 1,
  opacity: 1,
  nextCardOpacity: 0,
});
