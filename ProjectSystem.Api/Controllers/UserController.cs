using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectSystem.Api.Filters;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using ProjectSystem.Repositories.Contacts;
using ProjectSystem.Services.Contacts.Services;

namespace ProjectSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;

        public UserController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(CreatedUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUser(RegistrationUserRequest user, CancellationToken cancellationToken)
        {
            var response = await _authenticationServices.RegistrationUser(user, cancellationToken);
            return Ok(response);
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginUser(LoginUserRequest user, CancellationToken cancellationToken)
        {
            var response = await _authenticationServices.LoginUser(user, cancellationToken);
            return Ok(response);
        }
    }
}
