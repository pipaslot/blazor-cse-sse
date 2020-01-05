using System;

namespace Core.Jwt
{
    public sealed class JwtToken
    {
        public JwtToken(DateTime validTo, string value)
        {
            Value = value;
            ValidTo = validTo;
        }
        public DateTime ValidTo { get; set; }
        public string Value { get; set; }
    }
}