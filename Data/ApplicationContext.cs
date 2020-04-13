using GrandElementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<CarCategory> CarCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public ApplicationContext()
        {
            //Database.EnsureCreated();
        }
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conn = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            optionsBuilder.UseNpgsql(conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnCarCategoryModelCreating(modelBuilder);
        }

        protected void OnCarCategoryModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarCategory>()
                .ToTable("car_categories");
            modelBuilder.Entity<CarCategory>()
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<CarCategory>()
                .Property(c => c.Name).HasColumnName("name");
        }
        protected void OnProductModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .ToTable("products");
            modelBuilder.Entity<Product>()
                .Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Product>()
                .Property(c => c.Name).HasColumnName("name");
        }
    }
}
