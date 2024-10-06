using ProjectSystem.Domain.Models;

namespace ProjectSystem.Services.Contacts.Services
{
    public interface IAuthenticationServices
    {
        Task<CreatedUserResponse> RegistrationUser(RegistrationUserRequest request, CancellationToken cancellationToken);
        Task<LoginUserResponse> LoginUser(LoginUserRequest request, CancellationToken cancellationToken);

    }
}
