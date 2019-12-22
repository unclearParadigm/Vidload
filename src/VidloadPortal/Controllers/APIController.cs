using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VidloadCache;
using VidloadPortal.Models;
using VidloadPortal.Services;
using VidloadShared.Structures;
using VidloadShared.Models.Jobs;
using VidloadShared.Models.Cache;

namespace VidloadPortal.Controllers {
  public class APIController : Controller {
    private readonly IJobEnqueuer _jobEnqueuer;
    private readonly IVidloadCache _vidloadCache;

    public APIController(IJobEnqueuer jobEnqueuer, IVidloadCache vidloadCache) {
      _jobEnqueuer = jobEnqueuer;
      _vidloadCache = vidloadCache;
    }

    [HttpPost]
    public async Task<IActionResult> InitiateDownload(DownloadRequest downloadRequest) {
      if (!downloadRequest.IsValid())
        return Json(ResponseModel<object>.CreateFailure("The request is invalid and does not meet the API-Requirements"));

      var userId = "Anonymous";
      var traceId = Guid.NewGuid().ToString();
      var downloadLink = downloadRequest.DownloadLink.Trim();

      var downloadJobState = await _vidloadCache.GetJobStatus(downloadLink);
      if (downloadJobState.IsSuccess && downloadJobState.Value.HasValue) {
        return Json(ResponseModel<object>.CreateSuccess(null));
      }

      var existingMetaData = await _vidloadCache.GetMetadata(downloadLink);
      if (existingMetaData.IsSuccess && existingMetaData.Value.HasNoValue) {
        await _jobEnqueuer.Enqueue(new MediaMetadataJob {DownloadLink = downloadLink, TraceId = traceId, UserId = userId});
      }

      var existingFileLocation = await _vidloadCache.GetMediaLocation(downloadLink);
      if (existingFileLocation.IsSuccess && existingFileLocation.Value.HasValue) {
        await _jobEnqueuer.Enqueue(new MediaDownloadJob {DownloadLink = downloadLink, TraceId = traceId, UserId = userId});
      }

      await _vidloadCache.SetJobStatus(traceId, JobStatus.Enqueued);
      return Json(ResponseModel<object>.CreateSuccess(null));
    }

    [HttpGet]
    public async Task<IActionResult> MediaMetadata(string downloadLink) {
      if (string.IsNullOrWhiteSpace(downloadLink) || !Uri.TryCreate(downloadLink, UriKind.Absolute, out _))
        return Json(ResponseModel<MediaMetadata>.CreateFailure("Invalid Media URL"));

      var existingMetadataForDownloadLink = await _vidloadCache.GetMetadata(downloadLink);
      if (existingMetadataForDownloadLink.IsSuccess && existingMetadataForDownloadLink.Value.HasValue) {
        return Json(ResponseModel<MediaMetadata>.CreateSuccess(existingMetadataForDownloadLink.Value.Value));
      }

      return Json(ResponseModel<MediaMetadata>.CreateSuccess(null));
    }

    [HttpGet]
    public async Task<IActionResult> MediaLocation(string downloadLink) {
      if (string.IsNullOrWhiteSpace(downloadLink) || !Uri.TryCreate(downloadLink, UriKind.Absolute, out _))
        return Json(ResponseModel<MediaMetadata>.CreateFailure("Invalid Media URL"));

      var existingMetadataForDownloadLink = await _vidloadCache.GetMetadata(downloadLink);
      if (existingMetadataForDownloadLink.IsSuccess && existingMetadataForDownloadLink.Value.HasValue) {
        return Json(ResponseModel<MediaMetadata>.CreateSuccess(existingMetadataForDownloadLink.Value.Value));
      }

      return Json(ResponseModel<MediaMetadata>.CreateSuccess(null));
    }
  }
}
