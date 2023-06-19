using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace cartoraServer.models
{
    public class tags
    {
        //public tags()
        //{
        //}
        public Guid id { get; set; }
        public Boolean Vehicles { get; set; }
        [JsonIgnore]
        public Users? User { get; set; }
        public int UserId { get; set; }
        public Boolean? Properties { get; set; }
        public Boolean? Electronics { get; set; }
        public Boolean? Furnitures { get; set; }
        public Boolean? Health { get; set; }
        public Boolean? Fashion { get; set; }
        public Boolean? Sport { get; set; }
        public Boolean? Services { get; set; }
        public Boolean? Jobs { get; set; }
        public Boolean? Babies { get; set; }
        public Boolean? Agric { get; set; }
        public Boolean? Repairs { get; set; }
        public Boolean? Equipments { get; set; }
    }
}

