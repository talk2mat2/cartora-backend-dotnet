using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace cartoraServer.models
{
    [Index(nameof(email), IsUnique = true)]
    public class Users
    {
        //public Users
        //    ()
        //{
        //}
        public int id { get; set; }
        [Required]
        public string userName { get; set; } = "";
        public string brand { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        public string? country { get; set; }
        public DateTime? createdAt { get; set; } = DateTime.Now;
        public string? AboutMe { get; set; }
        [JsonIgnore]
        public List<Product> Products { get; set; } = null!;
        public string? profileImage { get; set; }
        [Required]
        public string password { get; set; } = "";
        public string? phoneNo { get; set; } 
        public tags? tags { get; set; }

    }
}

//{ "email":"Talking@yahoo.com",
//        "password":"hxhdndndndnddb",
//        "brand":"Bhdhdhdh","userName":
//        "Bxbdbdbdbdb","country":""}
