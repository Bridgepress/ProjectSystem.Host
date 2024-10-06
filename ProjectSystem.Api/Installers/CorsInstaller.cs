using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.Core.Installers;

namespace ProjectSystem.Api.Installers
{
    public class CorsInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: configuration["Cors:Name"]!,
                    policy =>
                    {
                        policy.WithOrigins(configuration.GetSection("Cors:Origins").Get<string[]>()!);
                        policy.AllowAnyHeader();
                        policy.AllowAnyMethod();
                    });
            });
        }
    }
}
