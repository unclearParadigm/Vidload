class VidloadController {

  constructor(backendApiService, videoUrlTextbox, formatDropdown, invalidParametersModal) {
    this.backendApiService = backendApiService;
    this.videoUrlTextbox = videoUrlTextbox;
    this.formatDropdown = formatDropdown;
    this.invalidParametersModal = invalidParametersModal;
  }

  initialize() {
    const self = this;
    this.formatDropdown.dropdown({
      action: function(text, value) {
        const url = self.videoUrlTextbox.val();
        if (self.isValidUrl(url)) {
          self.beginDownload(url, value.toLocaleLowerCase());
        }
      }
    });

    this.videoUrlTextbox.keypress(function (ev) {
      if (ev.keyCode === 13) {
        const url = self.videoUrlTextbox.val();
        const format = "mp3"; //Todo: read val from Dropdown

        if (self.isValidUrl(url)) {
          self.beginDownload(url, format);
        } else {
          self.invalidParametersModal.modal('show');
        }
      }
    });
  }

  beginDownload(downloadLink, format) {
    // todo: add job to history
    const downloadJob = new DownloadJob(new DownloadRequest(downloadLink, format), this.backendApiService);
    downloadJob.begin();
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
