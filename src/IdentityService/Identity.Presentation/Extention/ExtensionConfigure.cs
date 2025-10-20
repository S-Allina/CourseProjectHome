using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Identity.Application;
using Identity.Application.Dto;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Application.Validators;
using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Identity.Infrastructure;
using Identity.Infrastructure.DataAccess.Data;
using Identity.Infrastructure.Services;
using Identity.Presentation.Constants;
using Identity.Presentation.Initializer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net.Mail;
using System.Text;
using System.Text.Json.Serialization;


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
                    options.JsonSerializerOptions.IgnoreNullValues = true;
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

        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
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

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsConstants.PolicyName, policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:7004", "https://localhost:7052")
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = configuration["Auth:Google:ClientID"];
                options.ClientSecret = configuration["Auth:Google:ClientSecret"];
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;
                options.Events = new OAuthEvents
                {
                    OnRemoteFailure = context =>
                    {
                        // Здесь можно logged детали ошибки
                        context.Response.Redirect("/error");
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
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

        public static IServiceCollection AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var hangfireConnection = configuration.GetConnectionString(HangfireConstants.HangfireConnection);

            if (string.IsNullOrEmpty(hangfireConnection))
            {
                throw new ArgumentNullException(nameof(hangfireConnection),
                      "Hangfire connection string is not configured");
            }

            services.AddSingleton(new HangfireDbInitializer(hangfireConnection));

            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = HangfireConstants.CommandTimeout,
                    SlidingInvisibilityTimeout = HangfireConstants.CommandTimeout,
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                    PrepareSchemaIfNecessary = HangfireConstants.PrepareSchema
                }));

            services.AddHangfireServer(options =>
            {
                options.ServerName = HangfireConstants.HangfireServerName;
                options.Queues = new[] { DefaultQueue };
            });

            return services;
        }

        public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluentEmail(
                configuration[EmailConstants.SenderEmail],
                configuration[EmailConstants.Sender]);
            //.AddSmtpSender(new System.Net.Mail.SmtpClient()
            //{
            //    Host = configuration[EmailConstants.Host],
            //    Port = configuration.GetValue<int>(EmailConstants.Port),
            //    EnableSsl = configuration.GetValue<bool>(EmailConstants.EnableSsl, false),
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Timeout = EmailConstants.SmtpTimeout
            //});

            return services;
        }

        public static IServiceCollection AddConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(SwaggerConstants.JwtSettingsSection).Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = secretKey
                };
                o.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = ContentType;
                        var result = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            message = "You are not authorized to access this resource. Please authenticate."
                        });
                        return context.Response.WriteAsync(result);
                    },
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies[AccessTokenCookieName];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
