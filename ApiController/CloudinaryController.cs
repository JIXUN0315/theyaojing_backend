using System.Security.Cryptography;
using System.Text;
using Backend.EnvironmentOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.ApiController;

[ApiController]
[Route("api/[controller]")]
public class CloudinaryController: ControllerBase{
    private readonly CloudinaryOptions _cloudinaryOptions;


    public CloudinaryController(IOptions<CloudinaryOptions> options)
    {
        _cloudinaryOptions = options.Value;
    }
    

    [HttpGet("sign")]
    public IActionResult SignUpload(string folder)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        // 若你希望上傳的圖片都放在某資料夾，例如 blog_featured，可以加上 public_id 或 folder
        var parameters = new SortedDictionary<string, string>
        {
            { "timestamp", timestamp },
            { "folder", folder } 
        };

        // 組出 query string
        var toSign = string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        var hash = ComputeSha1Hash(toSign + _cloudinaryOptions.ApiSecret); // Cloudinary 規定：query + secret 做 SHA-1

        return Ok(new
        {
            signature = hash,
            timestamp = timestamp,
            apiKey = _cloudinaryOptions.ApiKey,
            cloudName = _cloudinaryOptions.CloudName,
            folder = folder
        });
    }

    private string ComputeSha1Hash(string input)
    {
        using var sha1 = SHA1.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha1.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}