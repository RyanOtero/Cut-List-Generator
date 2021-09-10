﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SolidPrice.Models;

namespace SolidPrice.Migrations
{
    [DbContext(typeof(CutListGeneratorContext))]
    partial class CutListGeneratorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.9");

            modelBuilder.Entity("SolidPrice.Models.CutItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Angle1")
                        .HasColumnType("float");

                    b.Property<float>("Angle2")
                        .HasColumnType("float");

                    b.Property<string>("AngleDirection")
                        .HasColumnType("text");

                    b.Property<string>("AngleRotation")
                        .HasColumnType("text");

                    b.Property<float>("Length")
                        .HasColumnType("float");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<int>("StickNumber")
                        .HasColumnType("int");

                    b.Property<int>("StockItemID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("StockItemID");

                    b.ToTable("CutItems");
                });

            modelBuilder.Entity("SolidPrice.Models.OrderItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<int>("StockItemID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("StockItemID");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetCutItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Length")
                        .HasColumnType("float");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<int>("SheetNumber")
                        .HasColumnType("int");

                    b.Property<int>("SheetStockItemID")
                        .HasColumnType("int");

                    b.Property<float>("Width")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.HasIndex("SheetStockItemID");

                    b.ToTable("SheetCutItems");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetOrderItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<int>("SheetStockItemID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("SheetStockItemID");

                    b.ToTable("SheetOrderItems");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetStockItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("CostPerSqFoot")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("ExternalDescription")
                        .HasColumnType("text");

                    b.Property<string>("Finish")
                        .HasColumnType("text");

                    b.Property<string>("InternalDescription")
                        .HasColumnType("text");

                    b.Property<int>("MatType")
                        .HasColumnType("int");

                    b.Property<float>("StockLengthInInches")
                        .HasColumnType("float");

                    b.Property<float>("StockWidthInInches")
                        .HasColumnType("float");

                    b.Property<float>("Thickness")
                        .HasColumnType("float");

                    b.Property<int?>("VendorID")
                        .HasColumnType("int");

                    b.Property<string>("VendorItemNumber")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("VendorID");

                    b.ToTable("SheetStockItems");
                });

            modelBuilder.Entity("SolidPrice.Models.StockItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("CostPerFoot")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("ExternalDescription")
                        .HasColumnType("text");

                    b.Property<string>("InternalDescription")
                        .HasColumnType("text");

                    b.Property<int>("MatType")
                        .HasColumnType("int");

                    b.Property<int>("ProfType")
                        .HasColumnType("int");

                    b.Property<float>("StockLength")
                        .HasColumnType("float");

                    b.Property<int?>("VendorID")
                        .HasColumnType("int");

                    b.Property<string>("VendorItemNumber")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("VendorID");

                    b.ToTable("StockItems");
                });

            modelBuilder.Entity("SolidPrice.Models.Vendor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("text");

                    b.Property<string>("ContactName")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("VendorName")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("SolidPrice.Models.CutItem", b =>
                {
                    b.HasOne("SolidPrice.Models.StockItem", "StockItem")
                        .WithMany()
                        .HasForeignKey("StockItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StockItem");
                });

            modelBuilder.Entity("SolidPrice.Models.OrderItem", b =>
                {
                    b.HasOne("SolidPrice.Models.StockItem", "StockItem")
                        .WithMany()
                        .HasForeignKey("StockItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StockItem");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetCutItem", b =>
                {
                    b.HasOne("SolidPrice.Models.SheetStockItem", "SheetStockItem")
                        .WithMany()
                        .HasForeignKey("SheetStockItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SheetStockItem");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetOrderItem", b =>
                {
                    b.HasOne("SolidPrice.Models.SheetStockItem", "SheetStockItem")
                        .WithMany()
                        .HasForeignKey("SheetStockItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SheetStockItem");
                });

            modelBuilder.Entity("SolidPrice.Models.SheetStockItem", b =>
                {
                    b.HasOne("SolidPrice.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("SolidPrice.Models.StockItem", b =>
                {
                    b.HasOne("SolidPrice.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorID");

                    b.Navigation("Vendor");
                });
#pragma warning restore 612, 618
        }
    }
}
