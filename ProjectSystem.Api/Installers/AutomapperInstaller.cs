using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.Core.Installers;

namespace ProjectSystem.Api.Installers
{
    public class AutomapperInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(ApiAssemblyMarker));
        }
    }
}
