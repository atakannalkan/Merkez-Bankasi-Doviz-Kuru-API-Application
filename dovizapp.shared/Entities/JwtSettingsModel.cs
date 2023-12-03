using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.shared.Entities
{
    public class JwtSettingsModel
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int ExpirationTimeByHour { get; set; }
    }
}