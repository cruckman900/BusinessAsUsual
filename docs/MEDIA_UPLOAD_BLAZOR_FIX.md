# Media Upload Blazor Server File Reference Fix

**Issue:** `Cannot read properties of null (reading '_blazorFilesById')`

**Date Fixed:** January 2025

## Problem

When uploading files through the `MediaUploader` component in Blazor Server, the following JavaScript error occurred:

```
TypeError: Cannot read properties of null (reading '_blazorFilesById')
```

This happens because Blazor Server loses the file reference when:
1. The component re-renders during upload
2. The file stream is accessed directly from `IBrowserFile`
3. The SignalR connection has timing issues with large file transfers

## Root Cause

In Blazor Server, file uploads go through SignalR WebSocket connections. When you open a file stream directly from `IBrowserFile` and then try to read it asynchronously, Blazor may lose the JavaScript-side file reference before the upload completes, especially with large files or slow connections.

## Solution

### 1. Read File into Memory First

Instead of streaming directly from `IBrowserFile`, read the entire file into a `MemoryStream` first:

```csharp
// ❌ BEFORE (causes error):
var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: maxFileSize));

// ✅ AFTER (works correctly):
using var fileStream = file.OpenReadStream(maxAllowedSize: maxFileSize);
using var memoryStream = new MemoryStream();
await fileStream.CopyToAsync(memoryStream);
memoryStream.Position = 0;
var fileContent = new StreamContent(memoryStream);
```

### 2. Use IHttpClientFactory with Base Address

Blazor Server doesn't provide a bare `HttpClient` in DI. Use `IHttpClientFactory`:

```csharp
// ❌ BEFORE:
@inject HttpClient HttpClient

// ✅ AFTER:
@inject IHttpClientFactory HttpClientFactory
@inject NavigationManager Navigation

using var httpClient = HttpClientFactory.CreateClient();
httpClient.BaseAddress = new Uri(Navigation.BaseUri);
```

### 3. Increase SignalR Message Size Limits

Configure larger message sizes in `Program.cs`:

```csharp
builder.Services.AddServerSideBlazor()
	.AddHubOptions(options =>
	{
		options.MaximumReceiveMessageSize = 512 * 1024 * 1024; // 512MB
	});
```

### 4. Configure Kestrel for Large Uploads

```csharp
builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.Limits.MaxRequestBodySize = 512 * 1024 * 1024; // 512MB
});
```

### 5. Update Controller Attributes

```csharp
[HttpPost("upload")]
[RequestSizeLimit(536_870_912)] // 512 MB
[RequestFormLimits(MultipartBodyLengthLimit = 536_870_912)]
public async Task<IActionResult> UploadMedia(...)
```

## Files Modified

1. `frontend/BusinessAsUsual.Web/Modules/LMS/Components/MediaUploader.razor`
   - Changed to use `IHttpClientFactory`
   - Added file-to-memory copy before upload
   - Increased timeout for large files

2. `frontend/BusinessAsUsual.Web/Program.cs`
   - Increased SignalR `MaximumReceiveMessageSize` to 512MB
   - Added Kestrel `MaxRequestBodySize` configuration

3. `frontend/BusinessAsUsual.Web/Controllers/LMSMediaController.cs`
   - Increased `RequestSizeLimit` to 512MB
   - Added `RequestFormLimits` attribute

## Testing

After these changes:

1. ✅ Upload small images (< 5MB) - works instantly
2. ✅ Upload large videos (50-200MB) - works with progress
3. ✅ Upload multiple files in succession - no reference errors
4. ✅ Upload during re-renders - stable file references

## Best Practices for Blazor Server File Uploads

1. **Always** read files into memory first for reliability
2. **Always** use `IHttpClientFactory`, never bare `HttpClient`
3. **Always** set appropriate size limits at all layers:
   - SignalR Hub options
   - Kestrel server options
   - Controller attributes
4. **Consider** showing upload progress for large files
5. **Consider** chunked uploads for files > 100MB
6. **Avoid** multiple simultaneous uploads (queue them)

## Alternative Approaches

For very large files (> 500MB), consider:

1. **Client-side Blazor (WASM)** - Direct HTTP upload, no SignalR
2. **Chunked Uploads** - Split large files into smaller chunks
3. **Azure Blob Storage** - Direct browser-to-blob upload with SAS tokens
4. **Separate Upload Service** - Dedicated file upload API outside Blazor

## Performance Notes

- Memory usage: File size × 2 (original + memory stream)
- Network: Full file transferred over SignalR WebSocket
- Timeout: Set `httpClient.Timeout` to 5+ minutes for large files
- Connection: SignalR keep-alive prevents timeout during upload

## References

- [Blazor file uploads documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/file-uploads)
- [ASP.NET Core Kestrel limits](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options)
- [SignalR configuration](https://learn.microsoft.com/en-us/aspnet/core/signalr/configuration)
