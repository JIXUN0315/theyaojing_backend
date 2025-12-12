using Backend.Dto;
using Backend.Repository;

namespace Backend.Services;

public class AuthService : IAuthService
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAdminUserRepository adminUserRepository, 
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _adminUserRepository = adminUserRepository;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginAsync(string username, string password)
    {
        try
        {
            // 檢查輸入參數
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "請輸入帳號和密碼"
                };
            }

            // 根據使用者名稱查詢使用者
            var adminUser = await _adminUserRepository.GetByUsernameAsync(username);
            if (adminUser == null)
            {
                _logger.LogWarning("Login attempt with non-existent username: {Username}", username);
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "帳號或密碼錯誤"
                };
            }

            // 驗證密碼
            var isPasswordValid = await _adminUserRepository.VerifyPasswordAsync(password, adminUser.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password attempt for username: {Username}", username);
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "帳號或密碼錯誤"
                };
            }

            // 產生 JWT Token
            var token = _jwtService.GenerateToken(adminUser);

            // 登入成功
            _logger.LogInformation("Successful login for username: {Username}", username);
            return new LoginResponseDto
            {
                Success = true,
                Message = "登入成功",
                Token = token,
                User = new AdminUserInfoDto
                {
                    Id = adminUser.Id,
                    Username = adminUser.Username,
                    Email = adminUser.Email
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for username: {Username}", username);
            return new LoginResponseDto
            {
                Success = false,
                Message = "登入時發生錯誤，請稍後再試"
            };
        }
    }
    
}