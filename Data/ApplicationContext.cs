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
        public DbSet<Car> Cars { get; set; }
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
            OnProductModelCreating(modelBuilder);
            OnCarModelCreating(modelBuilder);
        }
        protected void OnCarModelCreating(ModelBuilder mb) {

            mb.Entity<Car>(entity =>
            {
                entity.ToTable("cars");

                entity.HasComment("Перевозчики");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CarCategoryId)
                    .HasColumnName("car_category_id")
                    .HasComment("Категория машины");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("character varying")
                    .HasComment("Примечания к водителю");

                entity.Property(e => e.Contacts)
                    .HasColumnName("contacts")
                    .HasColumnType("character varying")
                    .HasComment("Контактные данные водителя");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.FreightPrice)
                    .HasColumnName("freight_price")
                    .HasColumnType("numeric");

                entity.Property(e => e.Owner)
                    .HasColumnName("owner")
                    .HasColumnType("character varying")
                    .HasComment("Владелец автомобиля");

                entity.Property(e => e.StateNumber)
                    .HasColumnName("state_number")
                    .HasColumnType("character varying")
                    .HasComment("гос. номер");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasColumnType("character varying")
                    .HasComment("Еденица измерения");

                entity.Property(e => e.Vat)
                    .HasColumnName("vat")
                    .HasComment("НДС(value added tax). 1-включен; 0-невключен");

                entity.HasOne(d => d.CarCategory)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.CarCategoryId)
                    .HasConstraintName("cars_car_categories_id_fk");
            });
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
