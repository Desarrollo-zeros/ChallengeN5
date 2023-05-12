﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Domain.Permissions.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("EmployeSurname")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("EmployeeForename")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("PermissionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("PermissionTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PermissionTypeId");

                    b.ToTable("permissions");
                });

            modelBuilder.Entity("Domain.Permissions.PermissionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("permissionType");
                });

            modelBuilder.Entity("Domain.Permissions.Permission", b =>
                {
                    b.HasOne("Domain.Permissions.PermissionType", "PermissionType")
                        .WithMany("Permissions")
                        .HasForeignKey("PermissionTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PermissionType");
                });

            modelBuilder.Entity("Domain.Permissions.PermissionType", b =>
                {
                    b.Navigation("Permissions");
                });
#pragma warning restore 612, 618
        }
    }
}
