class VidloadController {
  constructor(backendApiService, apiServiceUrls, videoUrlTextbox, formatDropdown, invalidParametersModal) {
    this.backendApiService = backendApiService;
    this.apiServiceUrls = apiServiceUrls;
    this.videoUrlTextbox = videoUrlTextbox;
    this.formatDropdown = formatDropdown;
    this.invalidParametersModal = invalidParametersModal;
  }

  initialize() {
    this.formatDropdown.dropdown({
      action: (text, value) => {
        const url = this.videoUrlTextbox.val();
        if (this.isValidUrl(url)) {
          this.beginDownload(url, value.toLocaleLowerCase());
        }
      }
    });

    this.videoUrlTextbox.keypress((ev) => {
      if (ev.keyCode === 13) {
        const url = this.videoUrlTextbox.val();
        const format = "mp3"; //Todo: read val from Dropdown

        if (this.isValidUrl(url)) {
          this.beginDownload(url, format);
        } else {
          this.invalidParametersModal.modal('show');
        }
      }
    });
  }

  beginDownload(downloadLink, format) {
    // todo: add job to history
    const downloadRequest = new DownloadRequest(downloadLink, format);
    const downloadJob = new DownloadJob(
      downloadRequest,
      this.backendApiService,
      (metadata) => this.mediaMetadataReceived(metadata),
      (location) => this.mediaLocationReceived(location),
      () => this.completed());

    /* Todo: inject element in controller constructor */
    $('.js-video-url-container').addClass('loading');
    $('.js-video-url-container').addClass('disabled');

    downloadJob.beginDownload();
  }

  mediaMetadataReceived(mediaMetadata) {
    $('.js-media-title').text(mediaMetadata.title);
    $('.js-media-description').text(mediaMetadata.description);
    $('.js-media-thumbnail').attr('src', mediaMetadata['thumbnail']);
    $('.js-video-source').attr('href', mediaMetadata['videoSource']);

    if (mediaMetadata['tags'] !== null) {
      for (let i = 0; i < mediaMetadata['tags'].length; i++) {
        $('.js-media-tags').append(`<a class=\"ui red label\">${mediaMetadata['tags'][i]}</a>`);
      }
    }
  }

  mediaLocationReceived(mediaLocation) {
    const player = new Plyr('.js-mediaplayer');
    player.source = {
      type: 'audio',
      title: 'gulasch todo',
      sources: [
        {
          src: `${this.apiServiceUrls.downloadUrl}?downloadLink=${mediaLocation['downloadLink']}`,
          type: 'audio/mp3',
        }
      ],
    };
  }

  completed() {
    /* Todo: inject element in controller constructor */
    $('.js-video-url-container').removeClass('loading');
    $('.js-video-url-container').removeClass('disabled');
  }

  isValidUrl(urlString) {
    const pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
      '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.)+[a-z]{2,}|' + // domain name
      '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
      '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
      '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
      '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
    return !!pattern.test(urlString);
  }
}
