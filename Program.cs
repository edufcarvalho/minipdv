using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;
using minipdv.Application.Services;
using minipdv.Application.UseCases.Auth;
using minipdv.Domain.Interfaces;
using minipdv.Domain.Rules;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Data.Repositories;
using minipdv.Infrastructure.Data.Seed;
#if WINDOWS
using minipdv.Presentation.Desktop.Forms.Shared;
#endif

namespace minipdv;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var runAsApi = args.Contains("--api")
            || Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH") is not null;

        if (runAsApi)
        {
            RunApiAsync(args).GetAwaiter().GetResult();
        }
#if WINDOWS
        else
        {
            RunWinForms();
        }
#else
        else
        {
            RunApiAsync(args).GetAwaiter().GetResult();
        }
#endif
    }

    static async Task RunApiAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dbConfig = DatabaseConfig.Load();

        builder.Services.AddDbContext<MiniPDVContext>(options =>
            options.UseSqlServer(dbConfig.ConnectionString));

        var jwtSecret = EnvConfig.Get("JWT_SECRET")
            ?? "MiniPDV_Dev_Secret_Key_Must_Be_At_Least_32_Chars";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.Claims
                            .FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

                        if (jti is not null)
                        {
                            var sessionRepo = context.HttpContext.RequestServices
                                .GetRequiredService<ISessionRepository>();

                            var session = await sessionRepo.GetByTokenAsync(jti);

                            if (session is null || session.IsRevoked || session.ExpiresAt <= DateTime.UtcNow)
                            {
                                context.Fail("Token revoked or expired");
                                return;
                            }

                            context.HttpContext.Items["SessionId"] = session.Id;
                        }
                    }
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();

        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<ISessionRepository, SessionRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<LoginUseCase>();
        builder.Services.AddScoped<RegisterUseCase>();
        builder.Services.AddScoped<LogoutUseCase>();
        builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
        builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();

        builder.Services.AddSingleton(sp =>
        {
            var context = sp.GetRequiredService<MiniPDVContext>();
            return new DatabaseInitializer(context, dbConfig);
        });

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            try
            {
                if (!initializer.IsDatabaseSeeded())
                    await initializer.SeedAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Database seeding skipped: {ex.Message}");
            }
        }

        app.MapControllers();
        await app.RunAsync();
    }

#if WINDOWS
    static void RunWinForms()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            var config = DatabaseConfig.Load();
            var options = new DbContextOptionsBuilder<MiniPDVContext>()
                .UseSqlServer(config.ConnectionString)
                .Options;

            using var context = new MiniPDVContext(options);

            var initializer = new DatabaseInitializer(context, config);

            if (!initializer.IsDatabaseSeeded())
                initializer.Seed();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database seeding failed: {ex.Message}",
                "MiniPDV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        System.Windows.Forms.Application.Run(new Form1());
    }
#endif
}
