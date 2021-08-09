﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Solidworks_Cutlist_Generator.Utils.Messenger;

namespace Solidworks_Cutlist_Generator.Models {
    internal class CutListGeneratorContext : DbContext {

        
        public static readonly LoggerFactory _myLoggerFactory = new LoggerFactory(new[] {
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
        });        public static string ConnectionString { get; set; }

        private static bool isMySQL;


        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CutItem> CutItems { get; set; }




            public CutListGeneratorContext(string connectionString) : base() {
            isMySQL = (bool)Application.Current.Properties["UseExternalDB"];
            ConnectionString = connectionString;
            try {
                ChangeTracker.LazyLoadingEnabled = false;
            } catch (Exception) {
                //fail silently
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockItem>()
                .HasOne(s => s.Vendor).WithMany();
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.StockItem).WithMany();
            modelBuilder.Entity<CutItem>()
                .HasOne(c => c.StockItem).WithMany();

            modelBuilder.Entity<StockItem>()
                .Navigation(s => s.Vendor)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<OrderItem>()
                .Navigation(o => o.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<CutItem>()
                .Navigation(c => c.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            if (isMySQL) {
                try {
                    optionsBuilder.UseMySQL(ConnectionString);
                    base.OnConfiguring(optionsBuilder);
                } catch (Exception) {
                    ErrorMessage("Database Error", "Please enter a connection string in the format of:\n" +
                        "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]");
                }
            } else {
                try {
                    optionsBuilder.UseSqlite(ConnectionString, option => {
                        option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    });
                    base.OnConfiguring(optionsBuilder);
                } catch (Exception e) {
                    string s = e.Message;
                    ErrorMessage("Database Error", "Please enter a connection string in the format of:\n" +
                        "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]");
                }

            }
        }
    }
}
