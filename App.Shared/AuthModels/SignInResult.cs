using Core.Jwt;

namespace App.Shared.AuthModels
{
    public class SingInResult
    {
        public bool Success { get; set; }
        public JwtToken AccessToken { get; set; }
        public string Username { get; set; }
    }
}