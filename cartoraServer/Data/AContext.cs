using System;
using Microsoft.EntityFrameworkCore;
namespace cartoraServer.Data;
using cartoraServer.models;

public class AContext:DbContext
{

    public AContext(DbContextOptions<AContext> options):base(options)
    {
     
    }
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<KnightModel> KnightModel { get; set; } = null!;
    public DbSet<LikeModel> LikeModel { get; set; }
    public DbSet<tags> Tags { get; set; }
    public DbSet<OtpModel> OtpModel { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity
    //}
}

