using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using minipdv.Application.DTOs.Auth;
using minipdv.Application.Interfaces;
using minipdv.Application.Services;
using minipdv.Application.Validators;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Domain.Rules;
using minipdv.Infrastructure.Configuration;
using minipdv.Infrastructure.Data.Context;
using minipdv.Infrastructure.Data.Repositories;
using minipdv.Infrastructure.Data.Seed;
using minipdv.Presentation.API.Middleware;

#if WINDOWS
using minipdv.Presentation.Desktop.Forms.Auth;
#endif

namespace minipdv;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var runAsApi = args.Contains("--api");

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

        var settings = new AppSettings();

        builder.Services.AddDbContext<MiniPDVContext>(options =>
            options.UseSqlServer(settings.ConnectionString,
                sql => sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSecret));

        builder.Services.AddSingleton(settings);

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

                        if (jti is null) return;

                        var sessionRepo = context.HttpContext.RequestServices
                            .GetRequiredService<ISessionRepository>();

                        var session = await sessionRepo.GetByTokenAsync(jti);

                        if (session is null || session.IsRevoked)
                        {
                            context.Fail("Token revoked");
                            return;
                        }

                        if (session.ExpiresAt <= DateTime.UtcNow)
                        {
                            await sessionRepo.RevokeAsync(session.Id);
                            context.Fail("Token expired");
                            return;
                        }
                        context.HttpContext.Items["Session"] = session;
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministrador", policy =>
                policy.RequireClaim("tipo", "Administrador"));
            options.AddPolicy("RequireFarmaceutico", policy =>
                policy.RequireClaim("tipo", "Farmaceutico", "Administrador"));
            options.AddPolicy("RequireAutenticado", policy =>
                policy.RequireClaim("tipo", "Usuario", "Farmaceutico", "Administrador"));
        });

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "MiniPDV API",
                Version = "v1",
                Description = "API do sistema MiniPDV para gestão farmacêutica."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT no formato: {seu token}"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddScoped<IAbstractUsuarioRepository, AbstractUsuarioRepository>();
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IFarmaceuticoRepository, FarmaceuticoRepository>();
        builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
        builder.Services.AddScoped<IContatoRepository, ContatoRepository>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped<IProdutoControladoRepository, ProdutoControladoRepository>();
        builder.Services.AddScoped<IProdutoGrupoRepository, ProdutoGrupoRepository>();
        builder.Services.AddScoped<IProdutoTipoRepository, ProdutoTipoRepository>();
        builder.Services.AddScoped<IFabricanteRepository, FabricanteRepository>();
        builder.Services.AddScoped<IPrincipioAtivoRepository, PrincipioAtivoRepository>();
        builder.Services.AddScoped<IProdutoCodBarraRepository, ProdutoCodBarraRepository>();
        builder.Services.AddScoped<IProdutoEstoqueRepository, ProdutoEstoqueRepository>();
        builder.Services.AddScoped<ISessionRepository, SessionRepository>();
        builder.Services.AddScoped<IPrescritorRepository, PrescritorRepository>();
        builder.Services.AddScoped<IReceitaRepository, ReceitaRepository>();
        builder.Services.AddScoped<IVendaRepository, VendaRepository>();
        builder.Services.AddScoped<IAdministradorRepository, AdministradorRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IFarmaceuticoService, FarmaceuticoService>();
        builder.Services.AddScoped<IClienteService, ClienteService>();
        builder.Services.AddScoped<IContatoService, ContatoService>();
        builder.Services.AddScoped<IProdutoService, ProdutoService>();
        builder.Services.AddScoped<IProdutoControladoService, ProdutoControladoService>();
        builder.Services.AddScoped<IProdutoGrupoService, ProdutoGrupoService>();
        builder.Services.AddScoped<IProdutoTipoService, ProdutoTipoService>();
        builder.Services.AddScoped<IFabricanteService, FabricanteService>();
        builder.Services.AddScoped<IPrincipioAtivoService, PrincipioAtivoService>();
        builder.Services.AddScoped<IProdutoCodBarraService, ProdutoCodBarraService>();
        builder.Services.AddScoped<IProdutoEstoqueService, ProdutoEstoqueService>();
        builder.Services.AddScoped<IPrescritorService, PrescritorService>();
        builder.Services.AddScoped<IReceitaService, ReceitaService>();
        builder.Services.AddScoped<IVendaService, VendaService>();
        builder.Services.AddScoped<IAdministradorService, AdministradorService>();
        builder.Services.AddTransient<IValidator<LoginRequest>, LoginRequestValidator>();
        builder.Services.AddTransient<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddTransient<IValidator<Usuario>, UsuarioValidator>();
        builder.Services.AddTransient<IValidator<Farmaceutico>, FarmaceuticoValidator>();
        builder.Services.AddTransient<IValidator<Cliente>, ClienteValidator>();
        builder.Services.AddTransient<IValidator<Contato>, ContatoValidator>();
        builder.Services.AddTransient<IValidator<Produto>, ProdutoValidator>();
        builder.Services.AddTransient<IValidator<ProdutoControlado>, ProdutoControladoValidator>();
        builder.Services.AddTransient<IValidator<ProdutoGrupo>, ProdutoGrupoValidator>();
        builder.Services.AddTransient<IValidator<ProdutoTipo>, ProdutoTipoValidator>();
        builder.Services.AddTransient<IValidator<Fabricante>, FabricanteValidator>();
        builder.Services.AddTransient<IValidator<PrincipioAtivo>, PrincipioAtivoValidator>();
        builder.Services.AddTransient<IValidator<ProdutoCodBarra>, ProdutoCodBarraValidator>();
        builder.Services.AddTransient<IValidator<ProdutoEstoque>, ProdutoEstoqueValidator>();
        builder.Services.AddTransient<IValidator<Prescritor>, PrescritorValidator>();
        builder.Services.AddTransient<IValidator<Receita>, ReceitaValidator>();
        builder.Services.AddTransient<IValidator<Venda>, VendaValidator>();
        builder.Services.AddTransient<IValidator<Administrador>, AdministradorValidator>();
        builder.Services.AddScoped<DatabaseInitializer>();

        var keysPath = EnvConfig.Get("DATA_PROTECTION_KEYS_PATH") ?? Path.Combine(AppContext.BaseDirectory, "keys");
        Directory.CreateDirectory(keysPath);
        builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(keysPath));

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<MiniPDVContext>();

            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await context.Database.MigrateAsync();
            });

            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await strategy.ExecuteAsync(async () =>
            {
                await initializer.SeedAsync();
                await initializer.SeedDataAsync();
            });
        }

        app.MapControllers();
        await app.RunAsync();
    }

#if WINDOWS
    static void RunWinForms()
    {
        ApplicationConfiguration.Initialize();

        var apiUrl = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";

        using var splash = new Form
        {
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.None,
            ControlBox = false,
            ShowInTaskbar = false,
            ClientSize = new Size(350, 120),
            BackColor = Color.White,
            TopMost = true
        };
        var lbl = new Label
        {
            Text = "Conectando ao servidor...",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.DarkBlue
        };
        splash.Controls.Add(lbl);

        var connected = false;
        splash.Shown += async (_, _) =>
        {
            using var http = new HttpClient { BaseAddress = new Uri(apiUrl.TrimEnd('/') + "/") };
            http.Timeout = TimeSpan.FromSeconds(5);

            for (int i = 0; i < 60; i++)
            {
                try
                {
                    var response = await http.GetAsync("api/health");
                    if (response.IsSuccessStatusCode)
                    {
                        connected = true;
                        break;
                    }
                }
                catch { }

                lbl.Text = $"Conectando ao servidor... ({i + 1}/60)";
                await Task.Delay(2000);
            }

            splash.Close();
        };

        System.Windows.Forms.Application.Run(splash);

        if (!connected)
        {
            System.Windows.Forms.MessageBox.Show(
                "Não foi possível conectar ao servidor API.\nVerifique se o servidor está em execução.",
                "Erro de Conexão",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
            return;
        }

        System.Windows.Forms.Application.Run(new LoginForm());
    }
#endif
}
