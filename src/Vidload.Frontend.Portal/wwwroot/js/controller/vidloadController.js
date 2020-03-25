class VidloadController {
  constructor(backendApiService, apiServiceUrls, inputComponent, outputComponent, mediaPlaybackComponent) {
    this.backendApiService = backendApiService;
    this.inputComponent = inputComponent;
    this.outputComponent = outputComponent;
    this.mediaPlaybackComponent = mediaPlaybackComponent;
    this.currentDownloadJob = null;
  }

  initialize() {
    this.outputComponent.hide();
    this.mediaPlaybackComponent.hide();
    this.inputComponent.initialize((mediaUrl, format) => this.beginDownload(mediaUrl, format));
  }

  beginDownload(downloadLink, format) {
    if(this.currentDownloadJob !== null)
      return; // do not start multiple downloads in parallel

    this.outputComponent.show();

    // todo: add job to history
    const downloadRequest = new DownloadRequest(downloadLink, format);

    this.currentDownloadJob = new DownloadJob(
      downloadRequest,
      this.backendApiService,
      (metadata) => this.mediaMetadataReceived(metadata),
      (location) => this.mediaLocationReceived(location),
      () => this.completed());

    this.currentDownloadJob.beginDownload();
  }

  mediaMetadataReceived(mediaMetadata) {
    this.outputComponent.show();
    this.outputComponent.renderMediaMetadata(mediaMetadata);
  }

  mediaLocationReceived(mediaLocation) {
    this.outputComponent.renderMediaLocation(mediaLocation);
    i
  }

  completed() {
    // Todo: Create Browser Notification (Notification API)
    this.inputComponent.initialize();
    this.currentDownloadJob = null;
  }
}
