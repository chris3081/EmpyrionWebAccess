﻿// <auto-generated />
using System;
using EmpyrionModWebHost.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmpyrionModWebHost.Migrations.Backpack
{
    [DbContext(typeof(BackpackContext))]
    partial class BackpackContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("EmpyrionModWebHost.Models.Backpack", b =>
                {
                    b.Property<string>("Id");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Content");

                    b.HasKey("Id", "Timestamp");

                    b.ToTable("Backpacks");
                });
#pragma warning restore 612, 618
        }
    }
}
