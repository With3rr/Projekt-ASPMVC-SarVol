using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SarVol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SarVol.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            

        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; } 
        public DbSet<Product> Products { get; set; }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Taste> Tastes { get; set; }

        




    }
}
