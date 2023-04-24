using System;
//using WebApi.Entities;
using cartoraServer.models;

namespace cartoraServer.services
{
    public class AuthResponse
    {
        public int id { get; set; }
        public string email { get; set; }
        public string brand { get; set; }
        public string userName { get; set; }
        public string Token { get; set; }
        public string profileImage { get; set; } = "";
        public dynamic createdAt { get; set; }
        public string AboutMe { get; set; }
        public string phoneNo { get; set; }


        public AuthResponse(Users user, string token)
        {
            id = user.id;
            email = user.email;
            brand = user.brand;
            userName = user.userName;
            Token = token;
            profileImage = user.profileImage!;
            createdAt = user.createdAt!;
            AboutMe = user.AboutMe= "";
            phoneNo = user.phoneNo = "";

        }
    }

}




