using Microsoft.EntityFrameworkCore;
using Solidworks_Cutlist_Generator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solidworks_Cutlist_Generator.ViewModels {
    internal class CutListGeneratorContext : DbContext {
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        public CutListGeneratorContext() : base() {
            //ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockItem>()
                .HasOne(s => s.Vendor).WithMany(v => v.StockItems);
            modelBuilder.Entity<StockItem>()
                .Navigation(s => s.Vendor)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<Vendor>()
                .Navigation(v => v.StockItems)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

            optionsBuilder.UseLazyLoadingProxies().UseMySQL("server=localhost;database=Cutlist Generator;user=root;password=password");
        }



    }
}
