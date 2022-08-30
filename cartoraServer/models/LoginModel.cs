using System;
using System.ComponentModel.DataAnnotations;

namespace cartoraServer.models
{
    public class LoginModel
    {
        public LoginModel()
        {
        }
        [Required]
        public string email { get; set; } = "";
        [Required]
        public string password { get; set; } = "";
    }
}

