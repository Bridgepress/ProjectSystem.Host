using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.Core.Installers;
using ProjectSystem.Services.Contacts.Services;
using ProjectSystem.Services.Implementation.Services;

namespace ProjectSystem.Services.Implementation.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
        }
    }
}
