using System;
namespace cartoraServer.services
{
    public class ResData
    {
        public string message { get; set; } = "";
        public bool status { get; set; } = false;
        public List<dynamic> data { get; set; } = new List<dynamic>(){};
    }
    
}

