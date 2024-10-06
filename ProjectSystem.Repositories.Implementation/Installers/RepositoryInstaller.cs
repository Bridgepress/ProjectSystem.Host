using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectSystem.Core.Installers;
using ProjectSystem.DataAccess;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Repositories.Contacts;
using ProjectSystem.Repositories.Contacts.Repositories;
using ProjectSystem.Repositories.Implementation.Repositories;

namespace ProjectSystem.Repositories.Implementation.Installers
{
    public class RepositoryInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8; 
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false; 
                options.Password.RequireLowercase = false;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            var repositories = typeof(RepositoryManager).Assembly.GetTypes()
                .Where(type => type.BaseType is not null && type.BaseType.IsGenericType &&
                               type.BaseType.GetGenericTypeDefinition() == typeof(RepositoryBase<>));

            foreach (var repository in repositories)
            {
                var repositoryInterface = repository.GetInterfaces()
                    .Single(i => !i.IsGenericType);
                services.AddScoped(repositoryInterface, repository);
            }
        }
    }
}
