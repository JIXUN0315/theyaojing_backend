using Backend.Dto;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.ApiController;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// 管理員登入
    /// </summary>
    /// <param name="loginRequest">登入請求</param>
    /// <returns>登入結果</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        try
        {
            // 記錄登入嘗試（不記錄密碼）
            _logger.LogInformation("Login attempt for username: {Username} from IP: {IpAddress}", 
                loginRequest.Username, 
                HttpContext.Connection.RemoteIpAddress?.ToString());

            var result = await _authService.LoginAsync(loginRequest.Username, loginRequest.Password);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for username: {Username}", loginRequest.Username);
            return StatusCode(500, new LoginResponseDto
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }
}