using System;
namespace cartoraServer.models
{
    public class knitedUsersList
    {

        public int? id { get; set; }
        public string? userName { get; set; }
        public string? brand { get; set; }
        public string? email { get; set; }
        public string? country { get; set; }
        public DateTime? createdAt { get; set; }
        public string? aboutMe { get; set; }
        public string? profileImage { get; set; }
        public string? phoneNo { get; set; }
        public Boolean? isKnigted { get; set; }

        public knitedUsersList(Users us)
        {
            id = us.id;
            userName = us.userName;
            brand = us.brand;
            email = us.email;
            country = us.country;
            createdAt = us.createdAt;
            aboutMe = us.AboutMe;
            profileImage = us.profileImage;
            phoneNo = us.phoneNo;

        }

  


    }
}

