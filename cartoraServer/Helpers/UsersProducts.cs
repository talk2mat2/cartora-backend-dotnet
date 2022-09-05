using System;
using cartoraServer.models;
namespace cartoraServer.Helpers
{
    public class UsersProducts
    {
        //public UsersProducts()
        //{

        //}
        public int id { get; set; }
        public string description { get; set; } = "";
        public bool? stock { get; set; } = true;

        public dynamic User { get; set; } = "";
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

