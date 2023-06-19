using System;
namespace cartoraServer.models
{
    public class KnightModel
    {
        //public KnightModel()
        //{
        //}

        public int id { get; set; }
        public int brandId { get; set; }
        public int folowerId { get; set; }
    }

    public record KnightIt2(int kniters, int knited);

    public record KnitersList(List<Users>  UsersList, int kniters);
}

