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

    
}

