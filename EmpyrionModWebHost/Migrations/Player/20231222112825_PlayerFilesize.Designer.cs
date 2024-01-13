﻿// <auto-generated />
using System;
using EmpyrionModWebHost.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EmpyrionModWebHost.Migrations
{
    [DbContext(typeof(PlayerContext))]
    [Migration("20231222112825_PlayerFilesize")]
    partial class PlayerFilesize
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("EmpyrionModWebHost.Models.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<float>("BodyTemp")
                        .HasColumnType("REAL");

                    b.Property<float>("BodyTempMax")
                        .HasColumnType("REAL");

                    b.Property<string>("BpInFactory")
                        .HasColumnType("TEXT");

                    b.Property<float>("BpRemainingTime")
                        .HasColumnType("REAL");

                    b.Property<int>("ClientId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Credits")
                        .HasColumnType("REAL");

                    b.Property<int>("Died")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EntityId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Exp")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("FactionGroup")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FactionId")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("FactionRole")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Filesize")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Food")
                        .HasColumnType("REAL");

                    b.Property<float>("FoodMax")
                        .HasColumnType("REAL");

                    b.Property<float>("Health")
                        .HasColumnType("REAL");

                    b.Property<float>("HealthMax")
                        .HasColumnType("REAL");

                    b.Property<int>("Kills")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastOnline")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Online")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OnlineHours")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("OnlineTime")
                        .HasColumnType("TEXT");

                    b.Property<byte>("Origin")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Oxygen")
                        .HasColumnType("REAL");

                    b.Property<float>("OxygenMax")
                        .HasColumnType("REAL");

                    b.Property<int>("Permission")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ping")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayerName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Playfield")
                        .HasColumnType("TEXT");

                    b.Property<float>("PosX")
                        .HasColumnType("REAL");

                    b.Property<float>("PosY")
                        .HasColumnType("REAL");

                    b.Property<float>("PosZ")
                        .HasColumnType("REAL");

                    b.Property<float>("Radiation")
                        .HasColumnType("REAL");

                    b.Property<float>("RadiationMax")
                        .HasColumnType("REAL");

                    b.Property<float>("RotX")
                        .HasColumnType("REAL");

                    b.Property<float>("RotY")
                        .HasColumnType("REAL");

                    b.Property<float>("RotZ")
                        .HasColumnType("REAL");

                    b.Property<string>("SolarSystem")
                        .HasColumnType("TEXT");

                    b.Property<float>("Stamina")
                        .HasColumnType("REAL");

                    b.Property<float>("StaminaMax")
                        .HasColumnType("REAL");

                    b.Property<string>("StartPlayfield")
                        .HasColumnType("TEXT");

                    b.Property<string>("SteamId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SteamOwnerId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Upgrade")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}