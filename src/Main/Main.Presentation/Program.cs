
using FluentValidation;
using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Application.Mapper;
using Main.Application.Services;
using Main.Application.Validators;
using Main.Domain.InterfacesRepository;
using Main.Infrastructure.DataAccess;
using Main.Infrastructure.DataAccess.Repositories;
using Main.Presentation.Constans;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Main.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryFieldRepository, InventoryFieldRepository>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IValidator<CreateInventoryDto>, CreateInventoryDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateInventoryFieldDto>,  CreateInventoryFieldDtoValidator>();
            builder.Services.AddAutoMapper(typeof(InventoryProfile));

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
                    c.RoutePrefix = "swagger";
                });
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
