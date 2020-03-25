class DownloadJob {
  constructor(downloadRequest, backendApiService, mediaMetadataCallback, mediaLocationCallback, completedCallback) {
    this.downloadRequest = downloadRequest;
    this.backendApiService = backendApiService;

    this.mediaMetadataCallback = mediaMetadataCallback;
    this.mediaLocationCallback = mediaLocationCallback;
    this.completedCallback = completedCallback;
    this.mediaMetadata = null;
    this.mediaLocation = null;
  }

  beginDownload() {
    this.backendApiService.initiateDownloadRequest(this.downloadRequest,  () => this.pollMediaMetadata());
  }

  pollMediaMetadata() {
    const self = this;
    if (this.mediaMetadata === null) {
      self.backendApiService.getMediaMetaData(
        this.downloadRequest,
        function (mediaMetadata) {
          self.setMediaMetadata(mediaMetadata);
          self.pollMediaLocation();
        },
        function () {
          setTimeout(() => self.pollMediaMetadata(), 1000);
        });
    }
  }

  setMediaMetadata(mediaMetadata) {
    this.mediaMetadata = mediaMetadata;
    this.mediaMetadataCallback(mediaMetadata);
  }

  pollMediaLocation() {
    const self = this;

    if (this.mediaLocation === null) {
      self.backendApiService.getMediaLocation(
        this.downloadRequest,
        function (mediaLocation) {
          self.setMediaLocation(mediaLocation);
          self.completeJob();
        },
        function () {
          setTimeout(() => self.pollMediaLocation(), 1000);
        });
    }
  }

  setMediaLocation(mediaLocation) {
    this.mediaLocationCallback(mediaLocation);
  }

  completeJob() {
    this.completedCallback();
  }
}
