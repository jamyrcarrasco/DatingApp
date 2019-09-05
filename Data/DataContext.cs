using DatingApp.APi.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.APi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
            
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
    }
}