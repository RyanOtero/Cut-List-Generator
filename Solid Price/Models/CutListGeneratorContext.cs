using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Windows;
using static Solid_Price.Utils.Messenger;

namespace Solid_Price.Models {
    internal class CutListGeneratorContext : DbContext {

        //comment out to scaffold
        private static bool _created = bool.Parse(Application.Current.Properties["IsCreated"].ToString());
        private static bool isMySQL;
        /////////////
        public string ConnectionString { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CutItem> CutItems { get; set; }



        public CutListGeneratorContext() { }

        public CutListGeneratorContext(string connectionString) : base() {

            //comment out to scaffold
            isMySQL = (bool)Application.Current.Properties["UseExternalDB"];
            ConnectionString = connectionString;
            if (!_created && !isMySQL) {

                Database.EnsureDeleted();
                Database.EnsureCreated();
                Application.Current.Properties["IsCreated"] = true;
                _created = true;
            }
            /////////////

            try {
                //ChangeTracker.LazyLoadingEnabled = false;
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
            //optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            optionsBuilder.EnableSensitiveDataLogging(true);

            //Uncomment to scaffold
            //try {
            //    optionsBuilder.UseMySQL("server=localhost;database=solid price;user=root;password=password");
            //    base.OnConfiguring(optionsBuilder);
            //} catch (Exception) {
            //    ErrorMessage("Database Error", "Please enter a connection string in the format of:\n" +
            //        "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]");
            //}
            //////////////

            //comment out to scaffold
            if (isMySQL) {
                try {
                    optionsBuilder.UseMySQL(ConnectionString);
                    base.OnConfiguring(optionsBuilder);
                } catch (Exception) {
                    ErrorMessage("Database Error cgc.cs 94", "Please enter a connection string in the format of:\n" +
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
                    ErrorMessage("Database Error cgc.cs 105", "Please enter a connection string in the format of:\n" +
                        "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]");
                }
            }
            ///////////////
        }
    }
}
