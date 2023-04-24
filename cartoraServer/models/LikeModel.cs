using System;
namespace cartoraServer.models
{
    public class LikeModel
    {
        //public LikeModel()
        //{
        //}

        public Guid id { get; set; }
        public int productId { get; set; }
        public int userId { get; set; }
    }
}

