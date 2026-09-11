using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/uploads")]
[Authorize]
public sealed class UploadsController : ControllerBase
{
    private const long MaximumFileSize =
        10 * 1024 * 1024;

    private static readonly HashSet<string>
        AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".pdf"
        };

    private readonly IWebHostEnvironment _environment;

    public UploadsController(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(
        typeof(UploadFileResult),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    public async Task<
        ActionResult<UploadFileResult>> Upload(
        [FromForm] UploadFileRequest request,
        CancellationToken cancellationToken)
    {
        var file = request.File;

        if (file is null || file.Length == 0)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Neispravan fajl",
                    Detail = "Izaberite fajl za upload.",
                    Status =
                        StatusCodes.Status400BadRequest
                });
        }

        if (file.Length > MaximumFileSize)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Fajl je prevelik",
                    Detail =
                        "Maksimalna veličina fajla je 10 MB.",
                    Status =
                        StatusCodes.Status400BadRequest
                });
        }

        var extension = Path.GetExtension(
            file.FileName);

        if (string.IsNullOrWhiteSpace(extension) ||
            !AllowedExtensions.Contains(extension))
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Nepodržan format",
                    Detail =
                        "Dozvoljeni formati su JPG, JPEG, PNG, WEBP i PDF.",
                    Status =
                        StatusCodes.Status400BadRequest
                });
        }

        var uploadsDirectory = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            "uploads");

        Directory.CreateDirectory(uploadsDirectory);

        var storedFileName =
            $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var filePath = Path.Combine(
            uploadsDirectory,
            storedFileName);

        await using (var stream = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            await file.CopyToAsync(
                stream,
                cancellationToken);
        }

        var relativeUrl =
            $"/uploads/{storedFileName}";

        var absoluteUrl =
            $"{Request.Scheme}://{Request.Host}{relativeUrl}";

        var result = new UploadFileResult(
            storedFileName,
            file.FileName,
            absoluteUrl,
            file.ContentType,
            file.Length);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }
}

public sealed class UploadFileRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}

public sealed record UploadFileResult(
    string FileName,
    string OriginalFileName,
    string Url,
    string ContentType,
    long Size);