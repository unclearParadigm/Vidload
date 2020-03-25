using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vidload.Library.Domain.Models.Cache;
using Vidload.Library.Domain.Models.Jobs;
using Vidload.Library.Domain.Structures;
using VidloadCache;
using VidloadPortal.Models;
using VidloadPortal.Services;

namespace VidloadPortal.Controllers {
  public class APIController : Controller {
    private readonly IJobEnqueuer _jobEnqueuer;
    private readonly IVidloadCache _vidloadCache;
    private readonly IHostingEnvironment _hostingEnvironment;

    public APIController(IJobEnqueuer jobEnqueuer, IVidloadCache vidloadCache, IHostingEnvironment hostingEnvironment) {
      _jobEnqueuer = jobEnqueuer;
      _vidloadCache = vidloadCache;
      _hostingEnvironment = hostingEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> InitiateDownload([FromBody] DownloadRequest downloadRequest) {
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
      if (existingFileLocation.IsSuccess && existingFileLocation.Value.HasNoValue) {
        var targetFormat = (OutputFormat)Enum.Parse(typeof(OutputFormat), downloadRequest.OutputFormat, true);
        await _jobEnqueuer.Enqueue(new MediaDownloadJob {DownloadLink = downloadLink, TraceId = traceId, UserId = userId, TargetFormat = targetFormat});
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
    public async Task<IActionResult> MediaLocation(string downloadLink, string outputFormat) {
      if (string.IsNullOrWhiteSpace(downloadLink) || !Uri.TryCreate(downloadLink, UriKind.Absolute, out _))
        return Json(ResponseModel<MediaLocation>.CreateFailure("Invalid Media URL"));

      var existingMetadataForDownloadLink = await _vidloadCache.GetMetadata(downloadLink);
      if (existingMetadataForDownloadLink.IsSuccess && existingMetadataForDownloadLink.Value.HasValue) {
        var mediaLocation = await _vidloadCache.GetMediaLocation(downloadLink);
        if (mediaLocation.IsSuccess && mediaLocation.Value.HasValue)
          return Json(ResponseModel<MediaLocation>.CreateSuccess(mediaLocation.Value.Value));
      }

      return Json(ResponseModel<MediaLocation>.CreateSuccess(null));
    }

    [HttpGet]
    public async Task<IActionResult> Download(string downloadLink) {
      var existingFileLocation = await _vidloadCache.GetMediaLocation(downloadLink);
      var filePath = existingFileLocation.Value.Value.FilePath;
      var fileContent = System.IO.File.ReadAllBytes(filePath);
      return File(fileContent, "application/force-download", downloadLink);
    }

    [ResponseCache(Duration = 60 * 60, Location = ResponseCacheLocation.Any, NoStore = true)]
    public IActionResult GetParticleConfiguration() {
      var relPath = Path.Join("wwwroot", "assets", "particlesConfiguration.json");
      var absPath = Path.Join(_hostingEnvironment.ContentRootPath, relPath);
      var fileContent = System.IO.File.ReadAllText(absPath, Encoding.UTF8);
      var deserialized = JsonConvert.DeserializeObject(fileContent);
      return Json(deserialized);
    }

    [HttpGet]
    public IActionResult SupportedAudioFormats() {
      return Json(ResponseModel<IList<OutputFormat>>.CreateSuccess(FormatSpecifier.AudioFormats));
    }
    
    [HttpGet]
    public IActionResult SupportedVideoFormats() {
      return Json(ResponseModel<IList<OutputFormat>>.CreateSuccess(FormatSpecifier.VideoFormats));
    }
  }
}
