using System.Data;
using Npgsql;

namespace Backend.DILibrary;

public static class RsdRegistration
{
    public static IServiceCollection AddRDS(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = System.Environment.GetEnvironmentVariable("SUPABASE_CONN")
                                   ?? config.GetConnectionString("DefaultConnection");
            NpgsqlConnection conn = new NpgsqlConnection();
            conn.ConnectionString = connectionString;
            return conn;
        });
        
        return services;
    }
}