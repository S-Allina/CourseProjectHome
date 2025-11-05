using Main.Application.Configuration;
using Main.Application.Hubs;
using Main.Presentation.MVC.Extention;
using Main.Presentation.MVC.Middleware;

namespace Main.Presentation.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.Configure<UrlSettings>(configuration.GetSection("Urls"));
            var urlSettings = configuration.GetSection("Urls").Get<UrlSettings>();
            var auth = configuration.GetSection("AuthSettings").Get<AuthSettings>();

            builder.Services
                .AddDatabaseContext(configuration)
                .AddCorsConfiguration(urlSettings)
                .AddAuthenticationConfiguration(urlSettings, auth)
                .AddSwaggerConfiguration(urlSettings)
                .AddHttpClientConfiguration(urlSettings)
                .AddApplicationServices()
                .AddApplicationRepositories()
                .AddApplicationValidators()
                .AddApplicationMapper()
                .AddMvcConfiguration();

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var supportedCultures = new[] { "en", "ru" };

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