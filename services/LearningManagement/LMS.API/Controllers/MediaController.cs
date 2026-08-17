using LMS.Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IBlobStorageService _blobStorage;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IBlobStorageService blobStorage, ILogger<MediaController> logger)
    {
        _blobStorage = blobStorage;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadMedia([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        try
        {
            using var stream = file.OpenReadStream();
            var url = await _blobStorage.UploadFileAsync(file.FileName, stream, file.ContentType);
            return Ok(new { url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            return StatusCode(500, "Error uploading file");
        }
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadMedia([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL is required");

        try
        {
            var stream = await _blobStorage.DownloadFileAsync(url);
            return File(stream, "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {Url}", url);
            return NotFound("File not found");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMedia([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL is required");

        try
        {
            await _blobStorage.DeleteFileAsync(url);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Url}", url);
            return StatusCode(500, "Error deleting file");
        }
    }
}
