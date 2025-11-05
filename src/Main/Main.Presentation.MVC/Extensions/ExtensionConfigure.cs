using FluentValidation;
using Main.Application.Configuration;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Interfaces;
using Main.Application.Mapper;
using Main.Application.Services;
using Main.Application.Validators;
using Main.Domain.InterfacesRepository;
using Main.Infrastructure.DataAccess;
using Main.Infrastructure.DataAccess.Repositories;
using Main.Infrastructure.ImgBBStorage;
using Main.Presentation.MVC.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Main.Presentation.MVC.Extention
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Scoped);

            return services;
        }

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, UrlSettings urlSettings)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsConstants.PolicyName, policy =>
                {
                    policy.WithOrigins(urlSettings.AuthFront, urlSettings.Auth, urlSettings.Main)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, UrlSettings urlSettings, AuthSettings auth)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthConstants.DefaultScheme;
                options.DefaultChallengeScheme = AuthConstants.ChallengeScheme;
            })
            .AddCookie(AuthConstants.DefaultScheme, options =>
            {
                options.LoginPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
            })
            .AddOpenIdConnect(AuthConstants.ChallengeScheme, options =>
            {
                options.Authority = urlSettings.Auth;
                options.ClientId = AuthConstants.ClientId;
                options.ClientSecret = auth.key;
                options.ResponseType = AuthConstants.ResponseType;
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SignedOutCallbackPath = AuthConstants.SignedOutCallbackPath;
                options.RemoteSignOutPath = AuthConstants.RemoteSignOutPath;

                options.Scope.Clear();
                foreach (var scope in AuthConstants.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.ClaimActions.MapUniqueJsonKey("theme", "theme");
                options.ClaimActions.MapUniqueJsonKey("language", "language");
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

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services, UrlSettings urlSettings)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerConstants.Version, new OpenApiInfo
                {
                    Title = SwaggerConstants.Title,
                    Version = SwaggerConstants.Version
                });

                c.AddSecurityDefinition(SwaggerConstants.SecurityScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{urlSettings.Auth}/connect/authorize"),
                            TokenUrl = new Uri($"{urlSettings.Auth}/connect/token"),
                            Scopes = SwaggerConstants.SecurityScopes.ToDictionary(s => s, s => s)
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
                                Id = SwaggerConstants.SecurityScheme
                            }
                        },
                        SwaggerConstants.SecurityScopes
                    }
                    });
            });

            return services;
        }

        public static IServiceCollection AddHttpClientConfiguration(this IServiceCollection services, UrlSettings urlSettings)
        {
            services.AddHttpClient(HttpClientConstants.AuthService, client =>
            {
                client.BaseAddress = new Uri(urlSettings.Auth);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IUrlService, UrlService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IInventoryStatsService, InventoryStatsService>();
            services.AddScoped<IImgBBStorageService, ImgBBStorageService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ICustomIdService, CustomIdService>();
            services.AddScoped<IItemService, ItemService>();

            return services;
        }

        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryFieldRepository, InventoryFieldRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateInventoryDto>, CreateInventoryDtoValidator>();
            services.AddScoped<IValidator<CreateInventoryFieldDto>, CreateInventoryFieldDtoValidator>();

            return services;
        }

        public static IServiceCollection AddApplicationMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(InventoryProfile));

            return services;
        }

        public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddControllers();
            services.AddOpenApi();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddControllersWithViews()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();

            return services;
        }
    }
}