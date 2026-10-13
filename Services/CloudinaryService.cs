using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace EchoVibe.Services;

public class CloudinaryService
{
    private readonly HttpClient _http;
    private readonly string _cloudName;
    private readonly string _uploadPreset;
    private readonly ILogger<CloudinaryService>? _logger;

    public CloudinaryService(HttpClient http, IConfiguration config, ILogger<CloudinaryService>? logger = null)
    {
        _http = http;
        _cloudName = config["CLOUDINARY_CLOUD_NAME"] ?? "";
        _uploadPreset = config["CLOUDINARY_UPLOAD_PRESET"] ?? "echovibe";
        _logger = logger;
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_cloudName) &&
        !string.IsNullOrWhiteSpace(_uploadPreset);

    public string? LastError { get; private set; }

    public async Task<string?> UploadAsync(IBrowserFile file, long maxBytes = 5 * 1024 * 1024)
    {
        LastError = null;

        if (!IsConfigured)
        {
            LastError = "Cloudinary is not configured (missing CLOUDINARY_CLOUD_NAME or UPLOAD_PRESET).";
            _logger?.LogWarning(LastError);
            return null;
        }

        var name = file.Name ?? "upload.jpg";
        var ext = Path.GetExtension(name).ToLowerInvariant();
        var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/jpg" };

        var typeOk = !string.IsNullOrEmpty(file.ContentType) &&
                     allowedTypes.Contains(file.ContentType.ToLowerInvariant());
        var extOk = allowedExt.Contains(ext);

        if (!extOk && !typeOk)
        {
            LastError = $"File type not allowed ({ext}, {file.ContentType}).";
            return null;
        }

        if (file.Size <= 0 || file.Size > maxBytes)
        {
            LastError = $"File size invalid ({file.Size} bytes). Max {maxBytes}.";
            return null;
        }

        try
        {
            await using var stream = file.OpenReadStream(maxBytes);
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(_uploadPreset), "upload_preset");

            var streamContent = new StreamContent(stream);
            var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(streamContent, "file", string.IsNullOrWhiteSpace(name) ? "photo.png" : name);

            var url = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload";
            using var response = await _http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                LastError = $"Cloudinary error {(int)response.StatusCode}: {json}";
                _logger?.LogWarning(LastError);
                return null;
            }

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("secure_url", out var secureUrl))
                return secureUrl.GetString();

            LastError = "Cloudinary response missing secure_url.";
            return null;
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            _logger?.LogError(ex, "Cloudinary upload exception");
            return null;
        }
    }
}