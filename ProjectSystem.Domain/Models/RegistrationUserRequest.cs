using ProjectSystem.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjectSystem.Domain.Models
{
    public class RegistrationUserRequest
    {
        [Required]
        public string userName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(FieldsValidation.User.PasswordMaxLength,
        ErrorMessage = "Password must be at least 8 characters long.",
        MinimumLength = FieldsValidation.User.PasswordMinLength)]
        public string password { get; set; }
    }
}
