using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TestDash.Models;

namespace TestDash.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CLSuser> Users { get; set; }
        public DbSet<CLSproject> Projects { get; set; }
        public DbSet<CLSclient> Clients { get; set; }
    }
}
