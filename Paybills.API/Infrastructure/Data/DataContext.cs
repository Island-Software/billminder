using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Paybills.API.Domain.Entities;

namespace Paybills.API.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        private IHostEnvironment _env;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DataContext(DbContextOptions<DataContext> options, IHostEnvironment env) : base(options)
        {
            _env = env;
        }

        public DataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured (e.g., by dependency injection or tests)
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var pgHost = Environment.GetEnvironmentVariable("PG_HOST");
            var pgPort = Environment.GetEnvironmentVariable("PG_PORT");
            var pgUser = Environment.GetEnvironmentVariable("PG_USER");
            var pgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD");
            var pgDb = Environment.GetEnvironmentVariable("PG_DB");

            var connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPassword};Database={pgDb};Ssl Mode=Require;Trust Server Certificate=true";

            optionsBuilder.UseNpgsql(connStr);

        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<BillType> BillTypes { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<ReceivingType> ReceivingTypes { get; set; }
        public DbSet<Receiving> Receivings { get; set; }

    }
}