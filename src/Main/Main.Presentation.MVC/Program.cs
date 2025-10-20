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
using Main.Presentation.MVC.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
                    policy.WithOrigins("http://localhost:5173", "https://localhost:7004", "https://localhost:7052")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = JwtConstants.ValidateIssuer,
                        ValidateAudience = JwtConstants.ValidateAudience,
                        ValidateLifetime = JwtConstants.ValidateLifetime,
                        ValidateIssuerSigningKey = JwtConstants.ValidateIssuerSigningKey,
                        ValidIssuer = configuration[JwtConstants.IssuerPath],
                        ValidAudiences = configuration[JwtConstants.AudiencePath].Split(JwtConstants.AudienceSeparator),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration[JwtConstants.KeyPath])),
                        ClockSkew = JwtConstants.ClockSkew
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api"))
                            {
                                context.Response.StatusCode = 401;
                                return Task.CompletedTask;
                            }
                            context.Response.Redirect("/auth/login");
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

                c.AddSecurityDefinition(SwaggerConstants.Scheme, new OpenApiSecurityScheme
                {
                    Description = SwaggerConstants.Description,
                    Name = SwaggerConstants.Name,
                    In = SwaggerConstants.In,
                    Type = SwaggerConstants.Type,
                    Scheme = SwaggerConstants.Scheme,
                    BearerFormat = SwaggerConstants.BearerFormat
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
            builder.Services.AddHttpClient("AuthService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5173"); // Адрес React приложения
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddEndpointsApiExplorer();
           
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryFieldRepository, InventoryFieldRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ICustomIdService, CustomIdService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            // Регистрируем сервис поиска
            builder.Services.AddScoped<ISearchRepository, SearchRepository>();

            // Остальные сервисы...
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IValidator<CreateInventoryDto>, CreateInventoryDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateInventoryFieldDto>, CreateInventoryFieldDtoValidator>();
            builder.Services.AddAutoMapper(typeof(InventoryProfile));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseMiddleware<AuthRedirectMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API V1");
                c.RoutePrefix = "swagger"; // Это делает доступным по /swagger/index.html
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
