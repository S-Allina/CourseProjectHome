using FluentValidation;
using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Application.Mapper;
using Main.Application.Services;
using Main.Application.Validators;
using Main.Domain.InterfacesRepository;
using Main.Infrastructure.DataAccess;
using Main.Infrastructure.DataAccess.Repositories;
using Main.Presentation.MVC.Constans;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Main.Presentation.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5173",
            "https://localhost:7004",
            "https://localhost:7052")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });

            // ✅ ПРАВИЛЬНАЯ настройка аутентификации
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/Account/Login"; // Локальный путь для логина
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // Адрес вашего IdentityServer
                options.Authority = "https://localhost:7052";
                options.ClientId = "MainMVCApp";
                options.ClientSecret = "your-secret"; // Должен совпадать с секретом в Auth

                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                // Настройка scope - ДОБАВЬТЕ "api1"
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("api1"); // ← ДОБАВЬТЕ ЭТОТ SCOPE

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("return_url", context.Properties.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerConstants.Version, new OpenApiInfo
                {
                    Title = SwaggerConstants.Title,
                    Version = SwaggerConstants.Version
                });

                // ✅ ОБНОВЛЕННАЯ настройка безопасности для OAuth2/OIDC
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:7052/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:7052/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID" },
                                { "profile", "Profile" },
                                { "email", "Email" }
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },
                        new[] { "openid", "profile", "email" }
                    }
                });
            });

            builder.Services.AddHttpClient("AuthService", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7052"); // ❌ ЗАМЕНИТЕ на адрес IdentityServer API
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddHttpContextAccessor();
            // Регистрация сервисов приложения
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryFieldRepository, InventoryFieldRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<ICustomIdService, CustomIdService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<ISearchRepository, SearchRepository>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IValidator<CreateInventoryDto>, CreateInventoryDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateInventoryFieldDto>, CreateInventoryFieldDtoValidator>();
            builder.Services.AddAutoMapper(typeof(InventoryProfile));

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // ✅ ДОБАВЬТЕ для обслуживания статических файлов
            app.UseRouting();

            app.UseCors("CorsPolicy");

            //app.UseMiddleware<AuthRedirectMiddleware>();
            // ✅ ПРАВИЛЬНЫЙ порядок middleware
            app.UseAuthentication(); // ДОЛЖЕН быть перед UseAuthorization
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API V1");
                c.RoutePrefix = "swagger";

                // ✅ Настройка OAuth для Swagger UI
                c.OAuthClientId("MainMVCApp");
                c.OAuthUsePkce();
            });

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}