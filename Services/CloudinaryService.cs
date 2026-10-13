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
        _cloudName = (config["CLOUDINARY_CLOUD_NAME"] ?? "").Trim();
        _uploadPreset = (config["CLOUDINARY_UPLOAD_PRESET"] ?? "echovibe").Trim();
        Console.WriteLine($"[Cloudinary] cloud={_cloudName} preset={_uploadPreset} configured={IsConfigured}");
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
            LastError = "Cloudinary not configured. Set CLOUDINARY_CLOUD_NAME and CLOUDINARY_UPLOAD_PRESET.";
            Console.WriteLine("[Cloudinary] " + LastError);
            return null;
        }

        if (file is null)
        {
            LastError = "No file selected.";
            return null;
        }

        Console.WriteLine($"[Cloudinary] Uploading name={file.Name} size={file.Size} type={file.ContentType}");

        if (file.Size <= 0 || file.Size > maxBytes)
        {
            LastError = $"File too large or empty ({file.Size} bytes). Max {maxBytes}.";
            return null;
        }

        try
        {
            await using var stream = file.OpenReadStream(maxBytes);
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(_uploadPreset), "upload_preset");

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType);
            content.Add(streamContent, "file", string.IsNullOrWhiteSpace(file.Name) ? "photo.jpg" : file.Name);

            var url = $"https://api.cloudinary.com/v1_1/{_cloudName}/image/upload";
            using var response = await _http.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Cloudinary] status={(int)response.StatusCode} body={json}");

            if (!response.IsSuccessStatusCode)
            {
                LastError = $"Cloudinary rejected upload ({(int)response.StatusCode}). Check preset is Unsigned.";
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
            Console.WriteLine("[Cloudinary] exception: " + ex);
            return null;
        }
    }
}