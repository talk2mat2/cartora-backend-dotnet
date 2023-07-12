using System;
namespace cartoraServer.models
{
    public class OtpModel
    {
        //public OtpModel()
        //{
        //}
        public int id { get; set; }
        public int Otp { get; set; }
        public string email { get; set; } = "";
        public DateTime Expires { get; set; }
    }
}

