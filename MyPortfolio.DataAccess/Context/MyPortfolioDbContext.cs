using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MyPortfolio.DataAccess.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPortfolio.DataAccess.Context
{
    public class MyPortfolioDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public MyPortfolioDbContext(DbContextOptions<MyPortfolioDbContext> options) : base(options)
        {
           
        }

        protected  override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new EntityConfigs.UserEntityConfiguration().Configure(modelBuilder.Entity<User>());
        }

    }
}
