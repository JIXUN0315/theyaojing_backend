using Backend.Dto;

namespace Backend.Services;

public interface IAuthService
{
    /// <summary>
    /// 管理員使用者登入
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <param name="password">密碼</param>
    /// <returns>登入結果</returns>
    Task<LoginResponseDto> LoginAsync(string username, string password);
}