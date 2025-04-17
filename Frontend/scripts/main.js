console.log("hello vue")
Vue.createApp({
    data() {
        return {
            message: 'Hello Vue!',
            names: [
                {
                    name: "test",
                    amount: 10,
                    description: "This is a test"
                },
                {
                    name: "flet",
                    amount: 20,
                    description: "Den här är flet, walla"
                },
                {
                    name: "bazinga",
                    amount: 99,
                    description: "Shuno e helt gone"
                }
            ],
            startX: 0,  // För att spara startpositionen av swipen
      offsetXPerName: [], // För att spara rörelseoffset för varje nameplate
      threshold: 50,
        };
    },
    setup() {
        const startX = Vue.ref(0);
        const offsetXPerName = Vue.ref([]); // Håller reda på varje nameplate's offset
        const threshold = 50; // Minsta distans för att godkänna swipe
    
        function startTouch(index, e) {
          startX.value = e.touches[0].clientX;
        }
    
        function moveTouch(index, e) {
          const currentX = e.touches[0].clientX;
          offsetXPerName.value[index] = currentX - startX.value;
        }
    
        function endTouch(index, e) {
          const endX = e.changedTouches[0].clientX;
          const distance = endX - startX.value;
    
          // Om boxen har dragits tillräckligt långt, göm den
          if (Math.abs(distance) > threshold) {
            offsetXPerName.value[index] += distance > 0 ? 100 : -100;
          } else {
            offsetXPerName.value[index] = 0; // Om dragningen inte är tillräcklig, återställ
          }
        }
    
        return {
          startTouch,
          moveTouch,
          endTouch,
          offsetXPerName,
          threshold
        };
      },

    methods: {
        handleSwipe(index, e) {
            const touch = e.touches ? e.touches[0] : e.changedTouches[0]; // Säkra att vi får rätt event beroende på start/move/end
            if (!touch) return;  // Om det inte finns någon touch, avbryt
      
      
            // Start touch
            if (e.type === "touchstart") {
              this.startX = touch.clientX;
              return;
            }
      
            // Move touch
            if (e.type === "touchmove") {
              const currentX = touch.clientX;
              this.offsetXPerName[index] = currentX - this.startX; // Uppdatera offset när du rör
              return;
            }
      
            // End touch
            if (e.type === "touchend") {
              const endX = touch.changedTouches[0].clientX;  // Få slutlig position
              const distance = endX - this.startX;
      
              // Om boxen har dragits tillräckligt långt, göm den
              if (Math.abs(distance) > this.threshold) {
                this.offsetXPerName[index] += distance > 0 ? 100 : -100; // Flytta bort om tillräcklig swipe
              } else {
                this.offsetXPerName[index] = 0; // Om inte, återställ positionen
              }
            }
          }
        
    },

    computed: {
        hasData() {
            return this.names.length > 0;
        }
    },
    watch: {
    },
    mounted() {
    }

}).mount('#app');