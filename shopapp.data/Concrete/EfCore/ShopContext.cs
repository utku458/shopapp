using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using shopapp.entity;
using Microsoft.EntityFrameworkCore;

namespace shopapp.data.Concrete.EfCore
{
    public class ShopContext : DbContext
    {
        
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {
        }
        
        public ShopContext() : base() 
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        
       
        public DbSet<Category> Categories { get; set; }

        public DbSet<Cart> Carts { get; set; }
        
        public DbSet<CartItem> CartItems { get; set; }


      


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
       
            if (!optionsBuilder.IsConfigured) // Eğer yapılandırılmamışsa uygula
            {
              
                optionsBuilder.UseSqlite("Data Source = shopapp.db");
            }
        }


        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {

            modelbuilder.Entity<ProductCategory>().HasKey(c => new{c.ProductId, c.CategoryId});
        }
    }

    
}