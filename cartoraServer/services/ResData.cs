using System;
namespace cartoraServer.services
{
    public class ResData<t>
    {
        public string message { get; set; } = "";
        public bool status { get; set; } = false;
        public List<t> data { get; set; } = new List<t>(){};
    }
    
}

