class DownloadJob {
  constructor(downloadRequest, backendApiService) {
    this.downloadRequest = downloadRequest;
    this.backendApiService = backendApiService;
    this.mediaMetadata = null;
    this.mediaLocation = null;
  }

  begin() {
    this.backendApiService.InitiateDownloadRequest(this.downloadRequest,  () => this.pollMediaMetadata());
  }

  pollMediaMetadata() {
    const self = this;
    if (this.mediaMetadata === null) {
      self.backendApiService.GetMediaMetaData(
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
    console.log("METADATA RECEIVED");
    this.mediaMetadata = mediaMetadata;
  }

  pollMediaLocation() {
    const self = this;

    if (this.mediaLocation === null) {
      self.backendApiService.GetMediaLocation(
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

  setMediaLocation() {
    console.log("MEDIALOCATION RECEIVED");
  }

  completeJob() {
    console.log("DOWNLOAD COMPLETED");
  }
}
