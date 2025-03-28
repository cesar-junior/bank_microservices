﻿// <auto-generated />
using System;
using BankMicroservices.Transfer.Model.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BankMicroservices.Transfer.Migrations
{
    [DbContext(typeof(MySQLContext))]
    partial class MySQLContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("BankMicroservices.Transfer.Model.TransferModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("Id"));

                    b.Property<float>("Amount")
                        .HasColumnType("float")
                        .HasColumnName("amount");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("ReceivedDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("received_date");

                    b.Property<string>("ReceiverUserId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("receiver_user_id");

                    b.Property<string>("SenderUserId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("sender_user_id");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sent_date");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("transfer");
                });
#pragma warning restore 612, 618
        }
    }
}
