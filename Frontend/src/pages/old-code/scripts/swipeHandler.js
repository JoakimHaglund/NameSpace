
function handleSwipe(event) {

    if (this.names.length === 0 || this.names[this.currentIndex] === undefined) {
      console.warn("Ingen data i names ännu eller fel index, chilla lite.");
      return;
    }
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
      this.position.startX = touch.clientX; 
      this.position.startY = touch.clientY;
    } else if (event.type === "touchmove") { //Client moves finger
      this.position.offsetX = touch.clientX - this.position.startX;
      this.position.offsetY = touch.clientY - this.position.startY;
      // Dynamisk rotation (lite sned om man drar i X-led)
      this.rotation = this.position.offsetX * 0.01; // Finjustera om du vill
      // Zoom ut när man swipar
      this.scale = 1 - Math.min(Math.abs(this.position.offsetX + this.position.offsetY), 200) / 1000;
      // Fade ut lite när man drar
      this.opacity = 1 - Math.min(Math.abs(this.position.offsetX + this.position.offsetY), 200) / 800;
    } else if (event.type === "touchend") { //Client lifts finger
      const direction = this.getDirection();
      console.log('Swipe direction:', direction);
      if (direction) {
        this.triggerSwipe(direction);
      }
      this.position.offsetX = 0;
      this.position.offsetY = 0;
      this.rotation = 0;
      this.opacity = 1;
      this.scale = 1;
    }

  };

  function getDirection() {
    console.log('OffsetX:', this.position.offsetX, 'OffsetY:', this.position.offsetY);
    if (Math.abs(this.position.offsetX) > Math.abs(this.position.offsetY)) {
      // Mer drag i X-led (vänster/höger vinner)
      if (this.position.offsetX < -this.swipeThreshold) {
        return 'left';
      } else if (this.position.offsetX > this.swipeThreshold) {
        return 'right';
      }
    } else {
      // Mer drag i Y-led (upp/ner vinner)
      if (this.position.offsetY < -this.swipeThreshold) {
        return 'up';
      } else if (this.position.offsetY > this.swipeThreshold) {
        return 'down';
      }
    }
    return null;
  };
  
  function triggerSwipe(direction) {
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
  }


