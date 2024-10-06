using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.Core.Installers;
using ProjectSystem.DataAccess.InitalDataCreate;

namespace ProjectSystem.DataAccess.Installers
{
    public class DataAccessInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(ApplicationDbContext.ConnectionStringKey),
                    sqlServerOptions => sqlServerOptions.MigrationsAssembly("ProjectSystem.DataAccess")));
            services.AddHostedService<MigrationsService>();
        }
    }
}
