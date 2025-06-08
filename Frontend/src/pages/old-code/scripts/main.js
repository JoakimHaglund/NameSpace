import * as api from './api.js';
import { state, lists, Display, Reaction, nameQuery, SubMenu } from './state.js';
import swipeCard from './swipeCard.js'
const { ref, computed, onMounted, reactive } = Vue;
const {position, nameplate, handleSwipe, getDirection} = swipeCard;
console.log("hello vue", nameplate);

Vue.createApp({
  setup() {
  //  const state = state
    // State vars, istället för data() och this
    
    const swipeItem = reactive({
      currentElement: null,
      offsetX: 0,
      initalLeft: 0
    });
    



    // Funktioner





   

    
    const toggleRegisterForm = () => {
      state.showRegisterForm = !state.showRegisterForm;
    };


    


    onMounted(() => {
        // Hämta query-param direkt från URL:n
        const params = new URLSearchParams(window.location.search);
        const token = params.get('token');  // Hämtar 'token' från URL:en
        const email = params.get('email');  // Hämtar 'token' från URL:en
        const partnerToken = params.get('partner-token');
        if (token && email){
          console.log('Token finns:', token);
          api.completeRegistration(token, email);
        }
        if (partnerToken) {
          console.log('partner-token finns:', partnerToken);
          api.comfirmPartner(partnerToken);
        }

      checkLoginStatus();

    });

   


    return {
      Display,
      SubMenu,
      sliderValue,
      nextCardStyle,
      scrollContainer,
      nameQuery,
      lists,
      shouldApplyOffsets,
      state,
      position,
      nameplate,
      horizontalSwipe,
      toggleSubmenu,
      toggleRegisterForm,
      onScroll,
      getTransform,
      resetDisplay,
      fetchData,
      login,
      logout,
      checkLoginStatus,
      handleSwipe,
      getGenderClass,
      getGenderIcon,
      register,
      getReactionSvg,
      sendPartnerRequest,
    };
  },
}).mount('#app');
