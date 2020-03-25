class MediaMetadataComponent {
  constructor(metadataContainer) {
    this.metadataContainer = metadataContainer;
  }


  hide() {
    this.metadataContainer.hide();
  }

  show() {
    this.metadataContainer.show();
  }

  renderMediaMetadata(metadata) {
    this.metadataContainer.find('.js-media-title').text(metadata.title);
    this.metadataContainer.find('.js-media-description').text(metadata.description);
    this.metadataContainer.find('.js-media-thumbnail').attr('src', metadata['thumbnail']);
    this.metadataContainer.find('.js-video-source').attr('href', metadata['videoSource']);

    if (metadata['tags'] !== null) {
      for (let i = 0; i < metadata['tags'].length; i++) {
        $('.js-media-tags').append(`<a class=\"ui red label\">${metadata['tags'][i]}</a>`);
      }
    }
  }

  renderMediaLocation(location) {
    console.log(location);
  }
}
