class MediaPlaybackComponent {
  constructor(playerContainer) {
    this.playerContainer = playerContainer;
    this.mediaPlayer = null;
  }

  hide() {
    this.playerContainer.hide();
  }
  
  show() {
    this.playerContainer.show();
  }
  
  playVideo(source, videoFormat, title) {
    this.playerContainer.find('audio').hide();
    this.playerContainer.find('video').show();
    
    const playerElement = this.playerContainer.find('video');
    this.mediaPlayer = new Plyr(playerElement);
    
    this.mediaPlayer.source = {
      type: 'video',
      title: title,
      sources: [{
        src: source,
        type: `video/${videoFormat.trim().toLowerCase()}`,
      }
      ]
    };
  }

  playAudio(source, audioFormat, title) {
    this.playerContainer.find('video').hide();
    this.playerContainer.find('audio').show();
    
    const playerElement = this.playerContainer.find('audio');
    this.mediaPlayer = new Plyr(playerElement);

    this.mediaPlayer.source = {
      type: 'audio',
      title: title,
      sources: [{
        src: source,
        type: `audio/${audioFormat.trim().toLowerCase()}`,
      }
      ]
    };
  }

}
