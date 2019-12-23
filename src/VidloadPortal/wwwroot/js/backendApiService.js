class BackendApiService {
  constructor(apiServiceUrls) {
    this.apiServiceUrls = apiServiceUrls;
  }

  GetMediaMetaData(mediaMetaDataRequest, responseWithDataCallback, responseWithoutDataCallback) {
    $.ajax({
      type: "GET",
      url: this.apiServiceUrls.getMediaMetadataUrl,
      data: JSON.stringify(mediaMetaDataRequest)
    }).done(function (data) {
      const response = JSON.parse(data);
      response['IsSuccess'] === true && response['HasData'] === true
        ? responseWithDataCallback(response)
        : responseWithoutDataCallback(response);
    });
  }

  GetMediaLocation(mediaLocationRequest, responseWithDataCallback, responseWithoutDataCallback) {
    $.ajax({
      type: "GET",
      url: this.apiServiceUrls.getMediaLocationUrl,
      data: JSON.stringify(mediaLocationRequest)
    }).done(function (data) {
      const response = JSON.parse(data);
      response['IsSuccess'] === true && response['HasData'] === true
        ? responseWithDataCallback(response)
        : responseWithoutDataCallback(response);
    });
  }

  InitiateDownloadRequest(downloadRequest, completedCallback) {
    $.ajax({
      type: "POST",
      url: this.apiServiceUrls.initiateDownloadUrl,
      data: JSON.stringify(downloadRequest)
    }).done(function () {
      completedCallback();
    });
  }
}
