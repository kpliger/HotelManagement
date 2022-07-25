using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HotelManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HotelManagement.Models.Item> Item { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Other model creating stuff here ...
            base.OnModelCreating(builder);

            //builder.Entity<Item>().Property(d => d.date_created).ValueGeneratedOnAdd();
            //builder.Entity<Item>().Property(d => d.date_updated).ValueGeneratedOnAddOrUpdate();

            //builder.Entity<Item>().Property(d => d.date_created).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            //builder.Entity<Item>().Property(d => d.date_created).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            //builder.Entity<Item>().Property(d => d.date_updated).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            //builder.Entity<Item>().Property(d => d.date_updated).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }


        public DbSet<HotelManagement.Models.ItemImage> ItemImage { get; set; }


        public DbSet<HotelManagement.Models.Employee> Employee { get; set; }


        public DbSet<HotelManagement.Models.Order> Order { get; set; }


        public DbSet<HotelManagement.Models.OrderItem> OrderItem { get; set; }
    }
}
