using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace EchoVibe.Services;

public class CloudinaryService
{
    private readonly HttpClient _http;
    private readonly string _cloudName;
    private readonly string _uploadPreset;

    public CloudinaryService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _cloudName = config["CLOUDINARY_CLOUD_NAME"] ?? "";
        _uploadPreset = config["CLOUDINARY_UPLOAD_PRESET"] ?? "echovibe";
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_cloudName) &&
        !string.IsNullOrWhiteSpace(_uploadPreset);

    /// <summary>
    /// Uploads an image to Cloudinary (unsigned preset). Returns the secure HTTPS URL, or null on failure.
    /// </summary>
    public async Task<string?> UploadAsync(IBrowserFile file, long maxBytes = 5 * 1024 * 1024)
    {
        if (!IsConfigured)
            return null;

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        if (!allowed.Contains(ext) || file.Size > maxBytes)
            return null;

        await using var stream = file.OpenReadStream(maxBytes);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(_uploadPreset), "upload_preset");

        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(streamContent, "file", file.Name);

        var url = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload";
        using var response = await _http.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Cloudinary upload failed: {response.StatusCode} {json}");
            return null;
        }

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("secure_url", out var secureUrl))
            return secureUrl.GetString();

        return null;
    }
}