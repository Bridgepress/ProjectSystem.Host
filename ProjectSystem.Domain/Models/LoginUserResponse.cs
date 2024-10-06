namespace ProjectSystem.Domain.Models
{
    public class LoginUserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public LoginUserResponse(Guid id, string userName, string accessToken, string refreshToken, string email)
        {
            Id = id;
            UserName = userName;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Email = email;
        }
    }
}
