import * as api from './api.js';

console.log("hello vue")
const Direction = Object.freeze({
  LEFT: 'left',
  RIGHT: 'right'
});

Vue.createApp({
  data() {
    return {
      isCheckingLogin: true, // ny flagga
      email: '',
      password: '',
      errorMessage: '',
      isLoggedIn: false,
      names: [],
      currentIndex: 0,
      offsetX: 0,
      startX: 0,
      offsetY: 0,
      startY: 0,
      swipeThreshold: 200,
      showNameplate: true, // för att hantera visning
      rotation: 0,
      scale: 1,
      opacity: 1,
      previousIndex: null,
    };
  },
  setup() {
  },

  methods: {
    logout() {
      // Här skickar vi en begäran till backend för att logga ut användaren
      axios.post('http://192.168.50.9:5228/api/Account/logout', {}, {
        withCredentials: true  // Skicka med cookien om den finns
      }).then(() => {
        this.isLoggedIn = false;  // Sätt statusen till icke inloggad
        // Valfritt: Töm eventuella andra användarrelaterade data som e-post, etc.
        this.email = '';
        this.password = '';
        // Redirect till login-sidan om du vill
        // window.location.href = '/login';  // Eller använd vue-router
      }).catch((error) => {
        console.error("Logout failed:", error);
      });
    },
    async login() {
      try {
        const response = await axios.post('http://192.168.50.9:5228/api/Account/login', {
          username: this.email,
          password: this.password,
        },{
          withCredentials: true, // Detta säkerställer att cookien skickas med i begäran
        });

        this.isLoggedIn = true;  // Uppdatera status till inloggad
        if (this.isLoggedIn){
          this.email = null
          this.password = null;
        }
        this.fetchData(); // Hämta namnplattor
      } catch (error) {
        this.errorMessage = 'Fel vid inloggning!';
        console.error('Login failed:', error);
      }
    },
    checkLoginStatus() {
        // Om cookien finns, sätt isLoggedIn till true
      // Backend kan kontrollera om JWT-token finns i cookien
      axios.get('http://192.168.50.9:5228/api/Account/check-login', {
        withCredentials: true,  // Skicka med cookien
      }).then((response) => {
        this.isLoggedIn = true;
      }).catch((error) => {
        this.isLoggedIn = false;
      }).finally(() => {    
        this.isCheckingLogin = false; // vi är klara, visa grejer
        console.log('isLoggedIn', this.isLoggedIn);
      });
    },

    async fetchData(){
      try {
        // Gör API-anropet här
        const response = await api.getData('a'); // Byt ut med din API-URL
        console.log(response); // Sätt datan i din names array
        this.names = response;
      } catch (error) {
        console.error('API-anropet misslyckades:', error);
      }
    },
    handleSwipe(event) {
      if (this.currentIndex < this.names.length - 1) {
        this.previousIndex = this.currentIndex + 1;
      }
      else {
        this.previousIndex = null;
      }
      event.preventDefault();
      const touch = event.touches?.[0] || event.changedTouches?.[0];
      console.log('touch event:', event.type);
      if (event.type === "touchstart") { //TouchEventStarts
        this.startX = touch.clientX;
        this.startY = touch.clientY;
      } else if (event.type === "touchmove") { //Client moves finger
        this.offsetX = touch.clientX - this.startX;
        this.offsetY = touch.clientY - this.startY;
        // Dynamisk rotation (lite sned om man drar i X-led)
        this.rotation = this.offsetX * 0.01; // Finjustera om du vill
        // Zoom ut när man swipar
        this.scale = 1 - Math.min(Math.abs(this.offsetX + this.offsetY), 200) / 1000;
        // Fade ut lite när man drar
        this.opacity = 1 - Math.min(Math.abs(this.offsetX + this.offsetY), 200) / 300;
      } else if (event.type === "touchend") { //Client lifts finger
        const direction = this.getDirection();
        console.log('Swipe direction:', direction);
        if (direction) {
          this.triggerSwipe(direction);
        }
        this.offsetX = 0;
        this.offsetY = 0;
        this.rotation = 0;
        this.opacity = 1;
        this.scale = 1;
      }

    },
    getDirection() {
      console.log('OffsetX:', this.offsetX, 'OffsetY:', this.offsetY);
      if (Math.abs(this.offsetX) > Math.abs(this.offsetY)) {
        // Mer drag i X-led (vänster/höger vinner)
        if (this.offsetX < -this.swipeThreshold) {
          return 'left';
        } else if (this.offsetX > this.swipeThreshold) {
          return 'right';
        }
      } else {
        // Mer drag i Y-led (upp/ner vinner)
        if (this.offsetY < -this.swipeThreshold) {
          return 'up';
        } else if (this.offsetY > this.swipeThreshold) {
          return 'down';
        }
      }
      return null;
    },
    triggerSwipe(direction) {
      if (direction === 'up') {
        console.log('TouchEvent: ' + direction);
      }
      else if (direction === 'down') {
        console.log('TouchEvent: ' + direction);
      }
      else if (direction === 'left') {
        console.log('TouchEvent: ' + direction);
      }
      else if (direction === 'right') {
        console.log('TouchEvent: ' + direction);
      }
      this.previousIndex = this.currentIndex;
      if (this.currentIndex < this.names.length - 1) {
        this.currentIndex++;

        // Ta bort gamla efter fade
        setTimeout(() => {
          this.previousIndex = null;
        }, 500);
      } else {
        // Sista kortet, visa inget mer
        console.log('Alla kort visade, nu gitta sista');
        this.showNameplate = false;
        this.previousIndex = null;

      }
    },


  },

  computed: {
    hasData() {
      return this.names.length > 0;
    }
  },
  watch: {
  },
  mounted() {
    this.checkLoginStatus(); 
    //this.fetchData();  // Hämtar data när komponenten laddas
  }

}).mount('#app');