using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions => Set<Transaction>();

    }
}