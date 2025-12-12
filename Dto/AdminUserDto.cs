namespace Backend.Dto;

public class AdminUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}

public class LoginRequestDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public AdminUserInfoDto? User { get; set; }
    
    public string Token { get; set; }
}

public class AdminUserInfoDto
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
}