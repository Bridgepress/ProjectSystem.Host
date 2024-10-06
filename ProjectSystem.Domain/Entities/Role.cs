using Microsoft.AspNetCore.Identity;

namespace ProjectSystem.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; } = default!;
    }
}
