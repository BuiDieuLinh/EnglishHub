using Microsoft.EntityFrameworkCore;
using EnglishHub.Domain.Entities;

namespace EnglishHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
}
