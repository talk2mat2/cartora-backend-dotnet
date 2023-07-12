using System;
namespace cartoraServer.models
{
    public class dtos
    {
        //public dtos()
        //{
        //}

        public record aboutmeDto(string aboutme);
        public record validateotpdto (int otp, string email);
        public record changepassDto(int otp, string newPsdd,string email);
    }
}

