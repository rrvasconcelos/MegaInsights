﻿// <auto-generated />
using System;
using MI.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MI.Infra.Data.Migrations
{
    [DbContext(typeof(MegaInsightsContext))]
    [Migration("20241212200914_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MI.Domain.Models.LotteryResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Accumulated")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ContestId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateOnly>("DrawDate")
                        .HasColumnType("date");

                    b.Property<int>("Result01")
                        .HasColumnType("int");

                    b.Property<int>("Result02")
                        .HasColumnType("int");

                    b.Property<int>("Result03")
                        .HasColumnType("int");

                    b.Property<int>("Result04")
                        .HasColumnType("int");

                    b.Property<int>("Result05")
                        .HasColumnType("int");

                    b.Property<int>("Result06")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("LotteryResults", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}