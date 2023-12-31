﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using api3.Models;

#nullable disable

namespace api3.Migrations
{
    [DbContext(typeof(PgAdminContext))]
    partial class PgAdminContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("api3.Models.Employee", b =>
                {
                    b.Property<int>("IdEmployee")
                        .HasColumnType("integer")
                        .HasColumnName("ID_Employee");

                    b.Property<string>("Name")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("IdEmployee")
                        .HasName("PK__Employee__D9EE4F362C37AD8C");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("api3.Models.Inventory", b =>
                {
                    b.Property<int>("IdInventory")
                        .HasColumnType("integer")
                        .HasColumnName("ID_Inventory");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Flavor")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.Property<int?>("IdEmployee")
                        .HasColumnType("integer")
                        .HasColumnName("ID_Employee");

                    b.Property<int?>("IdStore")
                        .HasColumnType("integer")
                        .HasColumnName("ID_Store");

                    b.Property<string>("IsSeasonFlavor")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)")
                        .HasColumnName("Is_season_flavor");

                    b.Property<int?>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("IdInventory")
                        .HasName("PK__Inventor__2210F49E6563AFD3");

                    b.HasIndex("IdEmployee");

                    b.HasIndex("IdStore");

                    b.ToTable("Inventory", (string)null);
                });

            modelBuilder.Entity("api3.Models.Store", b =>
                {
                    b.Property<int>("IdStore")
                        .HasColumnType("integer")
                        .HasColumnName("ID_Store");

                    b.Property<string>("Name")
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("IdStore")
                        .HasName("PK__Store__99D83D2C0FA271F8");

                    b.ToTable("Store", (string)null);
                });

            modelBuilder.Entity("api3.Models.Inventory", b =>
                {
                    b.HasOne("api3.Models.Employee", "IdEmployeeNavigation")
                        .WithMany("Inventories")
                        .HasForeignKey("IdEmployee")
                        .HasConstraintName("FK__Inventory__ID_Em__412EB0B6");

                    b.HasOne("api3.Models.Store", "IdStoreNavigation")
                        .WithMany("Inventories")
                        .HasForeignKey("IdStore")
                        .HasConstraintName("FK__Inventory__ID_St__403A8C7D");

                    b.Navigation("IdEmployeeNavigation");

                    b.Navigation("IdStoreNavigation");
                });

            modelBuilder.Entity("api3.Models.Employee", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("api3.Models.Store", b =>
                {
                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}
