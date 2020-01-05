using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Jwt
{
    public class AuthOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpirationInMinutes { get; set; }
    }
}
