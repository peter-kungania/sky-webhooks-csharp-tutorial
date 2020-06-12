using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkyWebhooksTutorial.AppSettings;

namespace SkyWebhooksTutorial
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Webhook service configuration.
            // These 6 lines are the only deviation from the standard .NET Core template Startup.cs
            services.Configure<WebhookConfig>(Configuration.GetSection("WebhookConfig"));
            services.AddSingleton<BusinessLogic.IWebhookService>((serviceProvider) =>
            {
                var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<BusinessLogic.WebhookService>();
                return new BusinessLogic.WebhookService(logger);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
