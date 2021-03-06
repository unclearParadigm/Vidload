class BackendApiService {
  constructor(apiServiceUrls) {
    this.apiServiceUrls = apiServiceUrls;
  }

  getMediaMetaData(mediaMetaDataRequest, responseWithDataCallback, responseWithoutDataCallback) {
    $.ajax({
      type: "GET",
      url: this.apiServiceUrls.getMediaMetadataUrl,
      data: {"downloadLink": mediaMetaDataRequest.downloadLink}
    }).done(function (response) {
      response['isSuccess'] === true && response['hasValue'] === true
        ? responseWithDataCallback(response['data'])
        : responseWithoutDataCallback();
    });
  }

  getMediaLocation(mediaLocationRequest, responseWithDataCallback, responseWithoutDataCallback) {
    $.ajax({
      type: "GET",
      url: this.apiServiceUrls.getMediaLocationUrl,
      data: {
        "downloadLink": mediaLocationRequest.downloadLink,
        "outputFormat": mediaLocationRequest.outputFormat
      }
    }).done(function (response) {
      response['isSuccess'] === true && response['hasValue'] === true
        ? responseWithDataCallback(response['data'])
        : responseWithoutDataCallback();
    });
  }

  initiateDownloadRequest(downloadRequest, completedCallback) {
    $.ajax({
      type: "POST",
      url: this.apiServiceUrls.initiateDownloadUrl,
      data: JSON.stringify(downloadRequest),
      dataType: "json",
      contentType: "application/json; charset=utf-8"
    }).done(function () {
      completedCallback();
    });
  }
}
