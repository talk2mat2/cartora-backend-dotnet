using System;
using System.ComponentModel;

namespace cartoraServer.models
{
    public enum Etypes
    {
        [Description("photo")]
        photo,
        [Description("video")]
        video,
        [Description("gif")]
        gif
    }

    

    public class ProductsDto
    {
        //public ProductsDto()
        //{
            
        //}

        
        public string description { get; set; } = "";
        public bool? stock { get; set; } = true;
        public List<string> frameColors { get; set; } = new List<string> { };
        //public string frameColors { get; set; } = "";
        public string? Snapshot { get; set; } = "";
        public string? Title { get; set; } = "";
        public Etypes? Mediatype { get; set; }
        public List<IFormFile>? File { get; set; }
        public int Price { get; set; }
        public bool Iscollection { get; set; } = false;
    }


}

