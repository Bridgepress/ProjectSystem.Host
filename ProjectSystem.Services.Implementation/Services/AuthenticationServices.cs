using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectSystem.Core.ConfigurationModels;
using ProjectSystem.Core.Exceptions;
using ProjectSystem.Domain.Entities;
using ProjectSystem.Domain.Models;
using ProjectSystem.Services.Contacts.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjectSystem.Services.Implementation.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtConfiguration> _jwtConfiguration;

        public AuthenticationServices(IMapper mapper, UserManager<User> userManager, 
            IOptions<JwtConfiguration> configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtConfiguration = configuration;
        }

        public async Task<LoginUserResponse> LoginUser(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName) ??
                  throw new NotFoundUserByUserNameException(request.UserName);
            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                throw new AuthorizationFailedException();
            }

            return await CreateToken(populateExp: true, user);
        }

        public async Task<CreatedUserResponse> RegistrationUser(RegistrationUserRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.email);
            if (existingUser != null)
            {
                throw new RegistrationException(new List<string> { "User with this email already exists" }, typeof(RegistrationUserRequest));
            }

            var user = _mapper.Map<User>(request);
            var result = await _userManager.CreateAsync(user, request.password);
            if (!result.Succeeded)
            {
                throw new RegistrationException(result.Errors.Select(x => x.Description).ToList(),
                    typeof(RegistrationUserRequest));
            }
            return new CreatedUserResponse { Id = user.Id };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private async Task<LoginUserResponse> CreateToken(bool populateExp, User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            if (populateExp)
            {
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            }

            await _userManager.UpdateAsync(user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new LoginUserResponse(user.Id, user.UserName, accessToken,
                refreshToken, user.Email);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfiguration.Value.TokenKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
            => new List<Claim>
                {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Sid, user.Id.ToString()),
                new(ClaimTypes.Expiration,
                    DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Value.Expires)).ToString("O"))
                }
                .Concat((await _userManager.GetRolesAsync(user))
                    .Select(role => new Claim(ClaimTypes.Role, role)))
                .ToList();

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtConfiguration.Value.ValidIssuer,
                audience: _jwtConfiguration.Value.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Value.Expires)),
                signingCredentials: signingCredentials);
        }
    }
}
