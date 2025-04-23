import * as api from './api.js';

const { watch, ref, computed, onMounted, reactive } = Vue;

console.log("hello vue");

Vue.createApp({
  setup() {
    // State vars, istället för data() och this
    const state = reactive({
      isCheckingLogin: true,
      isLoggedIn: false,
      showNameplate: false,
      showSelector: true,
      showFavorites: false,
      display: '',
      reactionsToAdd: [],
    });
    const position = reactive({
      offsetX: 0,
      startX: 0,
      offsetY: 0,
      startY: 0,
      EDGE_THRESHOLD: 120,
      SWIPE_THRESHOLD: 200,
    });
    const nameplate = reactive({
      names: [],

      currentIndex: 0,
      nextIndex: null,
      rotation: 0,
      scale: 1,
      opacity: 1
    });
    const nameQuery = reactive({
      letter: 'k',
      pagenum: 1,
    });
    const lists = reactive({
      favorites: [],
      liked: [],
      disliked: [],
      hasFetched: {
        favorites: false,
        liked: false,
        disliked: false,
      }
    });
    const rotation = ref(0);
    const scale = ref(1);
    const opacity = ref(1);

    // Funktioner
    const fetchData = async (letter, pagenum) => {
      try {
        const response = await api.getNamesByLetter(letter, pagenum); // Byt ut med din API-URL
        nameplate.names.push(...response);
        console.log("HERE: ", nameplate.names); // Sätt datan i din names array
        state.display = 'nameplate';
      } catch (error) {
        console.error('API-anropet misslyckades:', error);
      }
    };

    const fetchReactions = async (reaction) => {
      try {
        const response = await api.getReactions(reaction); // Byt ut med din API-URL
        if(reaction === 'favorites'){
          lists.favorites = [...response];
          lists.hasFetched.favorites = true;
        }
        else if (reaction === 'liked'){
          lists.liked = [...response];
          lists.hasFetched.liked = true;
        }
        else if (reaction === 'disliked'){
          lists.disliked = [...response];
          lists.hasFetched.disliked = true;
        }
        console.log("HERE: ", nameplate.names); // Sätt datan i din names array
      } catch (error) {
        console.error('API-anropet misslyckades:', error);
      }
    };

    const postReactions = async () => {
      const response = await api.addReactions(state.reactionsToAdd);
      console.log("reactionsToAdd: ", response);
lists.hasFetched.favorites = false;
      if (response.status === 200) {
        state.reactionsToAdd = [];
      } else {
        state.errorMessage = response;
      }
    };
    
    const checkLoginStatus = async () => {
      try {
        await axios.get('http://192.168.50.9:5228/api/Account/check-login', {
          withCredentials: true,
        });
        state.isLoggedIn = true;
        fetchData(nameQuery.letter, nameQuery.pagenum);
      } catch (error) {
        state.isLoggedIn = false;
      } finally {
        state.isCheckingLogin = false;
      }
    };

    const login = async (event) => {
      const response = await api.loginUser(event.target.email.value, event.target.password.value);
      console.log("Login: ", response);

      if (response.status === 200) {
        state.isLoggedIn = true;
        fetchData(nameQuery.letter, nameQuery.pagenum);
      } else {
        state.errorMessage = response;
      }
    };

    const logout = () => {
      axios.post('http://192.168.50.9:5228/api/Account/logout', {}, { withCredentials: true })
        .then(() => {
          state.isLoggedIn = false;
          // window.location.href = '/login';  // eller använd vue-router för redirect
        }).catch((error) => {
          console.error("Logout failed:", error);
        });
    };

    const handleSwipe = (event) => {
      if (nameplate.names.length === 0 || nameplate.names[nameplate.currentIndex] === undefined) {
        console.warn("Ingen data i names ännu eller fel index.");
        return;
      }
      if (nameplate.currentIndex < nameplate.names.length - 1) {
        nameplate.nextIndex = nameplate.currentIndex + 1;
      } else {
        nameplate.nextIndex = null;
      }

      const touch = event.touches?.[0] || event.changedTouches?.[0];
      //console.log('touch event:', event.type);

      if (event.type === "touchstart") {
        setStartPos(touch);
      } else if (event.type === "touchmove") {
        setOffsetPos(touch);
        const direction = getDirection();
        console.log('dir: ', null)
        if (direction != null && !direction.includes('from')) {
          setStylingParams();
        }
      } else if (event.type === "touchend") {
        const direction = getDirection();
        const passedThreshold = reachesThreshold();
        console.log('Swipe direction:', direction);
        if (passedThreshold && direction != null) {
          triggerSwipe(direction);
        }
        resetOffset();
        resetStylingParams();
      }
    };

    const getDirection = () => {
      const { startX, startY, offsetX, offsetY, EDGE_THRESHOLD } = position;
      const absX = Math.abs(offsetX);
      const absY = Math.abs(offsetY);
      //console.log('startX:', startX, 'startY:', startY, 'offsetX:', offsetX, 'offsetY:', offsetY, 'EDGE_THRESHOLD:', EDGE_THRESHOLD);

      const isHorizontal = absX > absY;
      const xEdgeLeft = startX <= EDGE_THRESHOLD;
      const xEdgeRight = startX >= window.innerWidth - EDGE_THRESHOLD;
      const yEdgeTop = startY <= EDGE_THRESHOLD;
      const yEdgeBottom = startY >= window.innerHeight - EDGE_THRESHOLD;
      //console.log('isHorizontal:', isHorizontal, 'xEdgeLeft:', xEdgeLeft, 'xEdgeRight:', xEdgeRight, 'yEdgeTop:', yEdgeTop, 'yEdgeBottom:', yEdgeBottom);

      // Om horisontell swipe
      if (isHorizontal) {
        if (offsetX < 0) return xEdgeRight ? 'from-right' : 'left';
        if (offsetX > 0) return xEdgeLeft ? 'from-left' : 'right';
      }

      // Om vertikal swipe
      if (!isHorizontal) {
        if (offsetY < 0) return yEdgeBottom ? 'from-down' : 'up';
        if (offsetY > 0) return yEdgeTop ? 'from-up' : 'down';
      }

      return null;
    };

    const reachesThreshold = () => {
      const absX = Math.abs(position.offsetX);
      const absY = Math.abs(position.offsetY);
      const passedX = absX > position.SWIPE_THRESHOLD;
      const passedY = absY > position.SWIPE_THRESHOLD;
      const passedThreshold = passedX || passedY;
      return passedThreshold;
    };

    const resetDisplay = () => {
      state.display = 'nameplate';
      console.log('clicked button')
    }

    const triggerSwipe = async (direction) => {

      console.log('TouchEvent:', direction, 'NamesLeft:', nameplate.names);
      if(direction.includes('from')){
        await postReactions();
      }
      switch (direction) {
        case 'from-up':
          fetchReactions('favorites')
          state.display = 'list-favorites';
          break;
        case 'up':
          addReaction(nameplate.names[nameplate.currentIndex].id, 1, true);
          break;
        case 'from-down':
          state.display = 'letter-wheel';
          break;
        case 'down':
          break;
        case 'from-left':
          fetchReactions('disliked')
          state.display = 'list-disliked';
          break;
        case 'left':
          addReaction(nameplate.names[nameplate.currentIndex].id, 0);
          break;
        case 'from-right':
          fetchReactions('liked')
          state.display = 'list-liked';
          break;
        case 'right':
          addReaction(nameplate.names[nameplate.currentIndex].id, 1);
          break;
      }
      console.log('reactions:', state.reactionsToAdd)
     
      if (!direction.includes('from')) {
        nameplate.names.shift();
        if (nameplate.names.length < 2) {
          nameQuery.pagenum++;
          console.log(nameQuery)
          postReactions();
          fetchData(nameQuery.letter, nameQuery.pagenum);
        }
        if (nameplate.names.length > 1) {
          nameplate.nextIndex = nameplate.currentIndex + 1;
        } else {
          nameplate.nextIndex = null;
        }
        if (nameplate.names.length === 0) {
          state.display = 'nameplate'
        }
      }


    };
    const addReaction = (nameInfoId, reaction, isAFavorite = false) => {
      if(isAFavorite){
        reaction = 1;
      }
      state.reactionsToAdd.push({ nameInfoId, reaction, isAFavorite });
    };

    const setStartPos = (touch) => {
      position.startX = touch.clientX;
      position.startY = touch.clientY;
    };
    const setOffsetPos = (touch) => {
      position.offsetX = touch.clientX - position.startX;
      position.offsetY = touch.clientY - position.startY;
    };
    const setStylingParams = () => {
      rotation.value = position.offsetX * 0.01;
      scale.value = 1 - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 1000;
      opacity.value = 1 - Math.min(Math.abs(position.offsetX + position.offsetY), 200) / 800;
    };
    const resetOffset = () => {
      position.offsetX = 0;
      position.offsetY = 0;
    };
    const resetStylingParams = () => {
      rotation.value = 0;
      opacity.value = 1;
      scale.value = 1;
    };

    const getGenderClass = (genderInt) => {
      const genderMap = {
        0: 'female',
        1: 'male',
      };
      return `gender-${genderMap[genderInt] || 'unknown'}`;
    };

    onMounted(() => {
      checkLoginStatus();
    });

    const shouldApplyOffsets = computed(() => {
      const dir = getDirection();
      return dir !== null && !dir.includes('from');
    });

   /* watch(
      () => [
        lists.favorites.length, 
        lists.hasFetched.favorites,
        lists.liked.length, 
        lists.hasFetched.liked,
        lists.disliked.length, 
        lists.hasFetched.disliked,
      ],
      ([favLen, favFetched, likedLen, LikedFetched, disLen, disFetched]) => {
        if (favLen === 0 && !favFetched) {
          fetchReactions('favorites')
        }
        if (likedLen === 0 && !LikedFetched) {
          fetchReactions('liked')
        }
        if (disLen === 0 && !disFetched) {
          fetchReactions('disliked')
        }
      },
      { immediate: true }
    );*/

    return {
      resetDisplay,
      lists,
      shouldApplyOffsets,
      state,
      position,
      nameplate,
      rotation,
      scale,
      opacity,
      fetchData,
      login,
      logout,
      checkLoginStatus,
      handleSwipe,
      setStartPos,
      setOffsetPos,
      setStylingParams,
      resetOffset,
      resetStylingParams,
      getGenderClass,
    };
  },
}).mount('#app');
