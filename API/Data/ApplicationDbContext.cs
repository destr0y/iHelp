using System;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Event> Events { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
