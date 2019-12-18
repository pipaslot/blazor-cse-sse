using System.ComponentModel.DataAnnotations;

namespace App.Shared.AuthModels
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