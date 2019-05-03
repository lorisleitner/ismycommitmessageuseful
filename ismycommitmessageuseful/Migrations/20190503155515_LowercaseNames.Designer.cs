﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ismycommitmessageuseful.Database;

namespace ismycommitmessageuseful.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20190503155515_LowercaseNames")]
    partial class LowercaseNames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ismycommitmessageuseful.Database.Commit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("DontKnowCount")
                        .HasColumnName("dont_know_count");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnName("message");

                    b.Property<int>("NotUsefulCount")
                        .HasColumnName("not_useful_count");

                    b.Property<int>("UsefulCount")
                        .HasColumnName("useful_count");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid");

                    b.HasKey("Id")
                        .HasName("commit_id_pkey");

                    b.ToTable("commit");
                });
#pragma warning restore 612, 618
        }
    }
}