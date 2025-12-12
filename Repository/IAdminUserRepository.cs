using Backend.Dto;

namespace Backend.Repository;

public interface IAdminUserRepository
{
    /// <summary>
    /// 根據使用者名稱取得管理員資料
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <returns>管理員資料，如果不存在則回傳 null</returns>
    Task<AdminUserDto?> GetByUsernameAsync(string username);

    /// <summary>
    /// 驗證密碼是否正確
    /// </summary>
    /// <param name="password">明文密碼</param>
    /// <param name="passwordHash">資料庫中的密碼雜湊</param>
    /// <returns>密碼是否正確</returns>
    Task<bool> VerifyPasswordAsync(string password, string passwordHash);
}