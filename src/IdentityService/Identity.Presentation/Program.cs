using Hangfire;
using Hangfire.Dashboard;
using Identity.Application.DTO;
using Identity.Domain.Entity;
using Identity.Infrastructure.DataAccess.Data;
using Identity.Presentation.Extensions;
using Identity.Presentation.Extention;
using Identity.Presentation.Initializer;
using Identity.Presentation.Middleware;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Identity.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            builder.Services.AddSingleton(emailSettings);
            builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.AddRazorPages();
            builder.Services.AddCustomCors();
            builder.Services.AddDatabaseConfiguration(configuration);
            builder.Services.AddControllerConfiguration();
            builder.Services.AddSwaggerConfiguration();
            builder.Services.AddIdentityConfiguration();
            //builder.Services.AddConfigureJwt(configuration);
            builder.Services.AddApplicationServices(configuration);
            builder.Services.AddAuthenticationConfiguration(configuration);
            builder.Services.AddEmailConfiguration(configuration);
            builder.Services.AddHangfireConfiguration(configuration);
            builder.Services.AddValidators();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            // ✅ ПРАВИЛЬНЫЙ порядок middleware для IdentityServer
            app.UseIdentityServer(); // ← ДОБАВЬТЕ ЭТУ СТРОЧКУ
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthFilter() },
                DashboardTitle = "Hangfire Dashboard",
                IgnoreAntiforgeryToken = true
            });

                app.UseEndpoints(endpoints =>
                {
                    // ✅ Убедитесь, что есть MapControllerRoute
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });

            app.MapRazorPages();

            await app.CreateDbIfNotExists();

            using (var scope = app.Services.CreateScope())
            {
                var hangfireInitializer = scope.ServiceProvider.GetRequiredService<HangfireDbInitializer>();
                await hangfireInitializer.EnsureDatabaseCreatedAsync();

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await DataDbInitializer.Initialize(userManager, roleManager);
            }

            app.Run();
        }
        }

    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}