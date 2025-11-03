using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using FluentValidation;
using Identity.Application.Configuration;
using Identity.Application.Dto;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Application.Validators;
using Identity.Domain.Entity;
using Identity.Infrastructure;
using Identity.Infrastructure.DataAccess.Data;
using Identity.Infrastructure.DataAccess.Services;
using Identity.Infrastructure.Services;
using Identity.Presentation.Constants;
using Identity.Presentation.Initializer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using Secret = Duende.IdentityServer.Models.Secret;

namespace Identity.Presentation.Extention
{
    public static class ServiceCollectionExtensions
    {
        private const string DefaultConnection = "DefaultConnection";
        private const string AccessTokenCookieName = "access_token";
        private const string DefaultQueue = "default";
        private const string ContentType = "application/json";

        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(DefaultConnection)));
            return services;
        }

        public static IServiceCollection AddControllerConfiguration(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddProblemDetails();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerConstants.SwaggerVersion, new OpenApiInfo
                {
                    Title = AuthPolicyConstants.Title,
                    Version = SwaggerConstants.SwaggerVersion,
                    Description = AuthPolicyConstants.Description
                });

                c.AddSecurityDefinition(SwaggerConstants.Scheme, new OpenApiSecurityScheme
                {
                    Name = SwaggerConstants.Name,
                    Type = SwaggerConstants.Type,
                    Scheme = SwaggerConstants.Scheme,
                    BearerFormat = SwaggerConstants.BearerFormat,
                    In = SwaggerConstants.In,
                    Description = SwaggerConstants.Description
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = SwaggerConstants.TypeReference,
                            Id = SwaggerConstants.Scheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });
            return services;
        }

        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var urlSettings = configuration.GetSection("Urls").Get<UrlSettings>();

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = AuthPolicyConstants.PasswordLength;
                o.SignIn.RequireConfirmedAccount = false;

                o.Lockout.AllowedForNewUsers = true;
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(AuthPolicyConstants.LockoutMinutes);
                o.Lockout.MaxFailedAccessAttempts = AuthPolicyConstants.MaxFailedAttempts;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(3);
            });

            // IdentityServer configuration
            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Authentication.CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;
                options.Authentication.CookieLifetime = TimeSpan.FromHours(1);
            })
            .AddInMemoryClients(new[]
            {
       new Client
    {
        ClientId = "MainMVCApp",
        ClientName = "Main MVC Application",
        ClientSecrets = { new Secret("your-secret".Sha256()) },
        AllowedGrantTypes = GrantTypes.Code,

        RedirectUris = { $"{urlSettings.Main}/signin-oidc" },
        PostLogoutRedirectUris = { $"{urlSettings.Main}/signout-callback-oidc" },
        FrontChannelLogoutUri = $"{urlSettings.Main}/signout-oidc",

 AllowedScopes = {
        "openid", "profile", "email", "api1", "roles",
        "theme", "language"
    },
        AllowAccessTokensViaBrowser = true,
        AlwaysIncludeUserClaimsInIdToken = true, 
        RequireConsent = false,
        RequirePkce = true,
        AllowPlainTextPkce = false,
        RequireClientSecret = true,
        UpdateAccessTokenClaimsOnRefresh = true,
        
        AlwaysSendClientClaims = true,
        ClientClaimsPrefix = "",
        IdentityProviderRestrictions = new List<string>
    {
        "Google",
        "Local"
    }
    } })
            .AddInMemoryIdentityResources(new List<IdentityResource>
            {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
        new IdentityResource("roles", "User roles", new[] { "role" }),
        new IdentityResource("theme", "User theme preference", new[] { "theme" }),
    new IdentityResource("language", "User language preference", new[] { "language" })
            })
            .AddInMemoryApiScopes(new List<ApiScope>
            {
        new ApiScope("api1", "My API"),
        new ApiScope("api.roles", "User roles for API"),
        new ApiScope("api.theme", "User theme preference"),
    new ApiScope("api.language", "User language preference")
            }).AddAspNetIdentity<ApplicationUser>()
.AddProfileService<CustomProfileService>() 
.AddDeveloperSigningCredential();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                // В продакшене используйте proper certificate
                // identityServerBuilder.AddSigningCredential(certificate);
            }

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var urlSettings = configuration.GetSection("Urls").Get<UrlSettings>();

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

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<ResetPasswordDto>, ResetPasswordValidator>();
            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IMainApiClient, MainApiClient>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMainApiClient, MainApiClient>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAutoMapper(typeof(UserMappingProfile));
            return services;
        }

        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = configuration["Auth:Google:ClientID"];
                    options.ClientSecret = configuration["Auth:Google:ClientSecret"];
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.CallbackPath = "/signin-google"; 
                    options.SaveTokens = true;
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                });

            return services;
        }

        public static async Task CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluentEmail(
                configuration[EmailConstants.SenderEmail],
                configuration[EmailConstants.Sender]);
          
            return services;
        }
    }
}
