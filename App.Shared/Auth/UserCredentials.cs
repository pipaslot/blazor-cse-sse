using System.ComponentModel.DataAnnotations;

namespace App.Shared.Auth
{
    public class UserCredentials
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [MinLength(4)]
        public string Password { get; set; }
    }
}