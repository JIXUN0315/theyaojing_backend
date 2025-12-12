using Backend.Repository;
using Backend.Services;

namespace Backend.DILibrary;

public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// Repository , Service 註冊
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        #region Service

        services.AddScoped<IEmailService, MailKitService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();


        #endregion
        #region repository

        services.AddScoped<IFormRepository,FormRepository>();
        services.AddScoped<IAdminUserRepository,AdminUserRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();

        #endregion
        return services;

    }
}