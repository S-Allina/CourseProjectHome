using FluentValidation;
using Main.Application.Configuration;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Hubs;
using Main.Application.Interfaces;
using Main.Application.Interfaces.ImgBBStorage;
using Main.Application.Mapper;
using Main.Application.Services;
using Main.Application.Validators;
using Main.Domain.InterfacesRepository;
using Main.Infrastructure.DataAccess;
using Main.Infrastructure.DataAccess.Repositories;
using Main.Infrastructure.ImgBBStorage;
using Main.Presentation.MVC.Constans;
using Main.Presentation.MVC.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace Main.Presentation.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            builder.Services.Configure<UrlSettings>(builder.Configuration.GetSection("Urls"));
            var urlSettings = configuration.GetSection("Urls").Get<UrlSettings>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins(urlSettings.AuthFront, urlSettings.Auth, urlSettings.Main)
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
                options.Authority = urlSettings.Auth;
                options.ClientId = "MainMVCApp";
                options.ClientSecret = "your-secret";

                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SignedOutCallbackPath = "/signout-callback-oidc";
                options.RemoteSignOutPath = "/signout-oidc";
                options.Scope.Clear(); 
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("api1");

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

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{urlSettings.Auth}/connect/authorize"),
                            TokenUrl = new Uri($"{urlSettings.Auth}/connect/token"),
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
                client.BaseAddress = new Uri(urlSettings.Auth);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryFieldRepository, InventoryFieldRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IUrlService, UrlService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IUserRepository,  UserRepository>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IInventoryStatsService, InventoryStatsService>();
            builder.Services.AddScoped<IImgBBStorageService, ImgBBStorageService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ICustomIdService, CustomIdService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<ISearchRepository, SearchRepository>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IValidator<CreateInventoryDto>, CreateInventoryDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateInventoryFieldDto>, CreateInventoryFieldDtoValidator>();
            builder.Services.AddAutoMapper(typeof(InventoryProfile));
            builder.Services.AddSignalR();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
            builder.Services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddCssBundle("/css/vendor.min.css",
                    "~/lib/bootstrap/dist/css/bootstrap.min.css",
                    "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css",
                    "https://cdn.datatables.net/1.13.6/css/dataTables.bootstrap5.min.css");

                pipeline.AddJavaScriptBundle("/js/vendor.min.js",
                    "~/lib/jquery/dist/jquery.min.js",
                    "~/lib/bootstrap/dist/js/bootstrap.bundle.min.js",
                    "https://cdn.datatables.net/1.13.6/js/jquery.dataTables.min.js");
            });
            var app = builder.Build();
            app.UseWebOptimizer();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var supportedCultures = new[] { "en", "ru", "by" };

            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API V1");
                c.RoutePrefix = "swagger";

                c.OAuthClientId("MainMVCApp");
                c.OAuthUsePkce();
            });

            app.MapStaticAssets();
            app.MapHub<ChatHub>("/chatHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}