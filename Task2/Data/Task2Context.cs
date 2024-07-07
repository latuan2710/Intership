using Microsoft.EntityFrameworkCore;
using Task2.Models;

namespace Task2.Data
{
    public class Task2Context : DbContext
    {
        public Task2Context(DbContextOptions<Task2Context> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<Role> Role { get; set; } = default!;
    }
}
