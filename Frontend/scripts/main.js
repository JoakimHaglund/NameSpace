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
    const svgFileLocations = reactive({
      gender: {
        female: '../resources/svgs/genders/female.svg',
        male: '../resources/svgs/genders/male.svg',
        unknown: '../resources/svgs/genders/unknown.svg',
      },
      reaction: {
        favorite: '../resources/svgs/reactions/favorite.svg',
        liked: '../resources/svgs/reactions/liked.svg',
        disliked: '../resources/svgs/reactions/disliked.svg',
        noReaction: '../resources/svgs/reactions/no-reaction.svg',
      },
    });
    
    const sliderValue = ref(50)

    const scrollContainer = ref(null);

    // Funktioner
    const fetchData = async (index, pagenum, isStartRequest = false, sliderValue) => {
      nameQuery.currentIndex = index;
      const letter = nameQuery.letters[nameQuery.currentIndex]
      console.log('index', index, nameQuery.currentIndex, 'letter', nameQuery.letters[nameQuery.currentIndex],  letter)
      //letter = letter === '?' ? nameQuery.letters[Math.floor(Math.random() * 29) + 1] : letter;
      try {
        const response = await api.getNamesByLetter(letter, pagenum, sliderValue); // Byt ut med din API-URL
        if(isStartRequest){
          nameplate.names = [...response]
        } else{

          nameplate.names.push(...response);
        }
        console.log("HERE: ", nameplate.names); // Sätt datan i din names array
        state.display = Display.CARD;
      } catch (error) {
        console.error('API-anropet misslyckades:', error);
      }
    };



    const checkLoginStatus = async () => {
      try {
        await axios.get('http://192.168.50.9:5228/api/Account/check-login', {
          withCredentials: true,
        });
        state.isLoggedIn = true;
        //fetchData(nameQuery.letter, nameQuery.pagenum);
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
        //fetchData(nameQuery.letter, nameQuery.pagenum);
      } else {
        state.errorMessage = response;
      }
    };
    const register = async (event) => {
      if (event.target.password.value === event.target.passwordRepeat.value){
        const response = await api.registerUser(event.target.username.value, event.target.email.value, event.target.password.value);
        console.log("Login: ", response);
        if (response.status === 200) {
          state.showRegisterForm = false;
  
        } else {
          state.errorMessage = response;
        }
      } else{
        state.errorMessage = 'Passwords do not match'
      }

    };
    const sendPartnerRequest = async (event) => {
      const partner = event.target.partner.value;
      if(partner !== null){
        const response = await api.addPartnerRequest(partner);
        console.log("Login: ", response);
        if (response.status === 200) {
          state.showRegisterForm = false;
  
        } else {
          state.errorMessage = response;
        }
      } else {
        state.errorMessage = 'no partner from submit';
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
    const getGenderIcon = (genderInt) => {
      if (genderInt === 0){
        return svgFileLocations.gender.female;
      }
      else if (genderInt === 1){
        return svgFileLocations.gender.male;
      }
      else {
        return svgFileLocations.gender.unknown;
      }
    };
    const getReactionSvg = (reaction) => {
      switch (reaction){
        case 2:
          return svgFileLocations.reaction.favorite;
        case 1: 
          return svgFileLocations.reaction.liked;
        case 0: 
          return svgFileLocations.reaction.disliked;
        default:
          return null;
      }
    }
    const getGenderClass = (genderInt) => {
      const genderMap = {
        0: 'female',
        1: 'male',
      };
      return `gender-${genderMap[genderInt] || 'unknown'}`;
    };
    const onScroll = () => {

      // Vänta 100ms innan scroll-beräkningen utförs

        const container = scrollContainer.value;
        const boxes = container.querySelectorAll('.box');

        const containerRect = container.getBoundingClientRect();
        const containerCenterX = containerRect.left + containerRect.width / 2;

        boxes.forEach((box) => {
          const rect = box.getBoundingClientRect();
          const boxCenterX = rect.left + rect.width / 2;

          // Beräkna avståndet från boxens mittpunkt till containerns mittpunkt
          const distanceFromCenter = Math.abs(boxCenterX - containerCenterX);

          // Maximal tolerans, hur mycket boxen kan vara bort från mitten innan den är som minst
          const maxDistance = containerRect.width / 2;
          
          // Beräkna skalan och fontstorleken baserat på avståndet från mitten
          let scale = 1 - (distanceFromCenter / maxDistance) * 0.3; // Max scale minskas med 30% ju längre från mitten
          scale = Math.max(scale, 0.7);
          const fontSize = Math.max(1.5, 27 * scale); // Justera den här formeln beroende på hur stor font du vill
          let distanceFactor = distanceFromCenter / maxDistance;
          let opacity = 1 - (0.8 * distanceFactor);
          
          if (opacity < 0.2) opacity = 0.2;
          console.log(opacity)
          // Applicera den beräknade skalan och fontstorleken
          box.style.transform = `scale(${scale})`;
          box.style.opacity = opacity;
          box.style.fontSize = `${fontSize}rem`;
         /* box.style.background = `radial-gradient(
            ellipse at center,
            rgb(255, 255, 255) 0%,
            rgba(0, 0, 255, 0) 70%
          )`;*/
        });
    };
    const getTransform = () => {
      return {
        transition: 'transform 0.1s ease, font-size 0.1s ease',
      };
    };
    const resetDisplay = () => {
      state.display = Display.CARD;
      console.log('clicked button')
    }
    const toggleRegisterForm = () => {
      state.showRegisterForm = !state.showRegisterForm;
    }
    const toggleSubmenu = (target) => {
      if (state.submenu === target){
        state.submenu = SubMenu.MAIN;
      }
      else {
        state.submenu = target;
      }
    }
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

    const shouldApplyOffsets = computed(() => {
      const dir = getDirection();
      return dir !== null && !dir.includes('from');
    });
    const nextCardStyle = computed(() => {
      return  { opacity: nameplate.nextCardOpacity }
       
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
