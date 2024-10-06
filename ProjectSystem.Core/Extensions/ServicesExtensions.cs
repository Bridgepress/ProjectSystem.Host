using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProjectSystem.Core.ConfigurationModels;
using ProjectSystem.Core.Installers;
using System.Reflection;
using System.Text;

namespace ProjectSystem.Core.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddInstallersFromAssemblies(this IServiceCollection services, IConfiguration configuration,
            IEnumerable<Assembly> assemblies)
        {
            assemblies.Distinct()
                .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsAssignableTo(typeof(IInstaller))))
                .Distinct()
                .Select(type => (IInstaller)Activator.CreateInstance(type)!)
                .OrderBy(InstallerOrder)
                .ToList()
                .ForEach(installer => installer.Install(services, configuration));
        }

        public static void AddInstallersFromAssemblies(this IServiceCollection services, IConfiguration configuration,
            params Assembly[] assemblies)
        {
            services.AddInstallersFromAssemblies(configuration, (IEnumerable<Assembly>)assemblies);
        }

        public static void AddInstallersFromAssemblies(this IServiceCollection services, IConfiguration configuration,
            params Type[] assemblyMarkers)
        {
            services.AddInstallersFromAssemblies(configuration,
                assemblyMarkers.Distinct().Select(assemblyMarker => assemblyMarker.Assembly));
        }

        private static int InstallerOrder(IInstaller installer)
        {
            if (installer is IOrderedInstaller orderedInstaller)
            {
                return orderedInstaller.Order;
            }

            return int.MinValue;
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfiguration = new JwtConfiguration();
            configuration.Bind(jwtConfiguration.Section, jwtConfiguration);
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfiguration.ValidIssuer,
                        ValidAudience = jwtConfiguration.ValidAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.TokenKey))
                    };
                });
        }

        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) =>
        services.Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));
    }
}
