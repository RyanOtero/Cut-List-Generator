using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using static SolidPrice.Utils.Messenger;

namespace SolidPrice.Models {
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
        public DbSet<SheetStockItem> SheetStockItems { get; internal set; }
        public DbSet<SheetCutItem> SheetCutItems { get; set; }

        public CutListGeneratorContext() { }

        public CutListGeneratorContext(string connectionString) : base() {

            //comment out to scaffold
            isMySQL = (bool)Application.Current.Properties["UseExternalDB"];
            ConnectionString = connectionString;
            if (!_created && !isMySQL) {
                try {
                    Directory.CreateDirectory(@"C:\ProgramData\Solid Price");
                    FileInfo fi;
                    if (!Debugger.IsAttached) {
                        fi = new FileInfo(@"C:\Program Files (x86)\Solid Price\CutList.db");
                    } else {
                        fi = new FileInfo(@"C:\Users\Ryan\source\repos\Solid Price\Solid Price\CutList.db");
                    }
                    fi.CopyTo(@"C:\ProgramData\Solid Price\CutList.db", true);
                    Application.Current.Properties["IsCreated"] = true;
                    _created = true;
                } catch (Exception e) {
                    ErrorMessage("Database Error cgc.cs 43", "Please enter a connection string in the format of:\n" +
                                "\nserver=[Server Name];database=[Database Name];user=[User Name];password=[Password]");
                }
            }
            /////////////
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<StockItem>()
                .HasOne(s => s.Vendor).WithMany();
            modelBuilder.Entity<SheetStockItem>()
               .HasOne(s => s.Vendor).WithMany();
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.StockItem).WithMany();
            modelBuilder.Entity<CutItem>()
                .HasOne(c => c.StockItem).WithMany();
            modelBuilder.Entity<SheetCutItem>()
                .HasOne(c => c.SheetStockItem).WithMany();

            modelBuilder.Entity<StockItem>()
                .Navigation(s => s.Vendor)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<OrderItem>()
                .Navigation(o => o.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<CutItem>()
                .Navigation(c => c.StockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<SheetStockItem>()
                .Navigation(s => s.Vendor)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
            modelBuilder.Entity<SheetCutItem>()
                .Navigation(c => c.SheetStockItem)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //optionsBuilder.UseLoggerFactory(_myLoggerFactory);
            //optionsBuilder.EnableSensitiveDataLogging(true);

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
                    ///////Release
                    optionsBuilder.UseSqlite(ConnectionString);
                    //optionsBuilder.UseSqlite(ConnectionString, option => {
                    //    option.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                    //});
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
