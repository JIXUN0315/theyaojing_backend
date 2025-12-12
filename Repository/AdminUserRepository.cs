using System.Data;
using Backend.Dto;
using Dapper;

namespace Backend.Repository;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly IDbConnection _db;

    public AdminUserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<AdminUserDto?> GetByUsernameAsync(string username)
    {
        const string sql = @"
            SELECT id, username, email, password_hash AS passwordhash
            FROM admin_users 
            WHERE username = @Username;
        ";

        return await _db.QueryFirstOrDefaultAsync<AdminUserDto>(sql, new { Username = username });
    }

    public async Task<bool> VerifyPasswordAsync(string password, string passwordHash)
    {
        const string sql = @"
            SELECT crypt(@Password, @PasswordHash) = @PasswordHash AS is_valid;
        ";

        var result = await _db.QueryFirstOrDefaultAsync<bool>(sql, new 
        { 
            Password = password, 
            PasswordHash = passwordHash 
        });

        return result;
    }
}