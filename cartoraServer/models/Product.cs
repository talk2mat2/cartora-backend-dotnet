using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace cartoraServer.models
{
    //public enum Etypes
    //{
    //    photo,
    //    video,
    //    gif
    //}
    public class ImgeUrll
    {
       public int id { get; set; }
        public string url { get; set; }="";
        [JsonIgnore]
        public Product? Product { get; set; }
        public int Productid { get; set; }
    }
    public class Likes
    {
        public int id { get; set; }
        public string userName { get; set; } = "";
        public int UserId { get; set; }
        public int ProductId { get; set; }

    }
    public class Product
    {
        //public Products()
        //{
            
        //}

        public int id { get; set; }
        public string description { get; set; } = "";
        public bool? stock { get; set; } = true;
        
        public Users? User { get; set; }
        public int UserId { get; set; }
        public string frameColors { get; set; } = "";
        public DateTime createdAt { get; set; } = DateTime.Now;
        public string? Snapshot { get; set; } = "";
        public Etypes? Mediatype { get; set; }
        public List<ImgeUrll> Media { get; set; } = new List<ImgeUrll>();
        public int Price { get; set; }
        public bool Iscollection { get; set; } = false;

    }

   
}

