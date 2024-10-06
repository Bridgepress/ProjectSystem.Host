using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProjectSystem.Core.Installers;

namespace ProjectSystem.Api.Installers
{
    public class ControllersInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
            => services.AddControllers()
                .AddApplicationPart(typeof(ApiAssemblyMarker).Assembly)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
    }
}
