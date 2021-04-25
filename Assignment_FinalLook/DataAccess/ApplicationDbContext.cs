using Microsoft.EntityFrameworkCore;
using Assignment_FinalLook.Models;

namespace Assignment_FinalLook.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Park> Park { get; set; }
        public DbSet<Activities> Activities { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<Users> Users { get; set; }

    }
}