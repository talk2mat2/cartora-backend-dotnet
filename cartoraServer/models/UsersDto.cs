using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace cartoraServer.models
{
    [Index(nameof(email), IsUnique = true)]
    public class UsersDto
    {
        //public UsersDto()
        //{
        //}
        public int id { get; set; }
        [Required]
        public string userName { get; set; } = "";
        public string brand { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        public string? country { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; } = DateTime.Now;
        public string? profileImage { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = "";
       
    }
}

//{ "email":"Talking@yahoo.com",
//        "password":"hxhdndndndnddb",
//        "brand":"Bhdhdhdh","userName":
//        "Bxbdbdbdbdb","country":""}
