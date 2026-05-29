using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pfstr.Api.Models;

namespace Pfstr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController(IConfiguration config) : ControllerBase
{
    private static readonly HashSet<string> AllowedTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp"];

    private static readonly HashSet<string> AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    [HttpPost]
    [Authorize]
    [RequestSizeLimit(10_485_760)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10_485_760)]
    public async Task<ActionResult<AssetResponse>> Upload(IFormFile file)
    {
        if (!AllowedTypes.Contains(file.ContentType))
            return BadRequest(new { error = "Only JPEG, PNG, GIF and WebP images are allowed" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { error = "Unsupported file extension" });

        var uploadsPath = Path.GetFullPath(
            config["Uploads:Path"] ?? Path.Combine(AppContext.BaseDirectory, "uploads"));
        Directory.CreateDirectory(uploadsPath);

        var filename = $"{Guid.NewGuid()}{ext}";
        await using var stream = System.IO.File.Create(Path.Combine(uploadsPath, filename));
        await file.CopyToAsync(stream);

        return Ok(new AssetResponse($"/static/assets/{filename}"));
    }
}
