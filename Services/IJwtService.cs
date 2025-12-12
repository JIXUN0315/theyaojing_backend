using System.Security.Claims;
using Backend.Dto;

namespace Backend.Services;

public interface IJwtService
{
    /// <summary>
    /// 產生 JWT Token
    /// </summary>
    /// <param name="adminUser">管理員使用者資訊</param>
    /// <returns>JWT Token</returns>
    string GenerateToken(AdminUserDto adminUser);

    /// <summary>
    /// 驗證 JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>使用者資訊，如果 Token 無效則回傳 null</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// 從 Token 中取得使用者 ID
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>使用者 ID</returns>
    string? GetUserIdFromToken(string token);
}