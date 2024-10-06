using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSystem.Core.ConfigurationModels
{
    public class JwtConfiguration
    {
        public string Section { get; init; } = "JwtSettings";

        public string ValidIssuer { get; set; } = default!;

        public string ValidAudience { get; set; } = default!;

        public string Expires { get; set; } = default!;

        public string TokenKey { get; set; } = default!;

        public int RefreshTokenExpiresDays { get; set; }
    }
}
