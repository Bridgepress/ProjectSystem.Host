using ProjectSystem.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjectSystem.Domain.Models
{
    public class LoginUserRequest
    {
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password name is required")]
        [StringLength(FieldsValidation.User.PasswordMaxLength,
               ErrorMessage = "Password must be at least 8 characters long.",
               MinimumLength = FieldsValidation.User.PasswordMinLength)]
        public string Password { get; set; }
    }
}
