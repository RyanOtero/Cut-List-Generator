using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.Models {
    internal class CutListGeneratorContext : DbContext {
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CutItem> CutItems { get; set; }

        public CutListGeneratorContext() : base() {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockItem>()
                .HasOne(s => s.Vendor).WithMany(v => v.StockItems);
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.StockItem);
            modelBuilder.Entity<CutItem>()
                .HasOne(c => c.StockItem);
            modelBuilder.Entity<StockItem>()
                .Navigation(s => s.Vendor)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<Vendor>()
                .Navigation(v => v.StockItems)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<OrderItem>()
                .Navigation(o => o.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<CutItem>()
                .Navigation(c => c.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }

        public static readonly LoggerFactory _myLoggerFactory =
    new LoggerFactory(new[] {
        new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
    });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            optionsBuilder.UseMySQL("server=localhost;database=Cutlist Generator;user=root;password=password");
        }



    }
}
