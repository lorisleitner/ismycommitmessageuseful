using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ismycommitmessageuseful.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Commit> Commits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Commit>(builder =>
            {
                builder
                    .HasKey(c => c.Id)
                    .HasName("commit_id_pkey");

                builder
                    .ForNpgsqlUseXminAsConcurrencyToken();
            });
                
        }
    }
}
