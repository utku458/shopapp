using System.Linq;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore;

public static class SeedDatabase
{
    public static void Seed()
    {
        var context = new ShopContext();
        
            if (context.Categories.Count() == 0)
            {
                context.Categories.AddRange(Categories);
            }

            if (context.Products.Count() == 0)
            {
                context.Products.AddRange(Products);
                context.AddRange(ProductCategories);
            }
        
         context.SaveChanges();
    }





    private static Category[] Categories =
    {
        new Category() { Name = "Telefon",Url = "telefon"},
        new Category() { Name = "Bilgisayar" ,Url = "bilgisayar"},
        new Category() { Name = "Elektronik",Url = "elektronik" },
        new Category() { Name = "Diger" ,Url = "diger" },
    };
    
    
    
    private static Product[] Products =
    {
        new Product() {Name = "Samsung S24", Price = 20000 ,Url = "samsung-s24",ImageUrl = "samsungs24.png",Description = "Samsung Telefon",IsApproved = true,IsHome = true},
        new Product() {Name = "Iphone 11", Price = 15000 ,Url = "iphone-11",ImageUrl = "iphone11.png",Description = "Samsung Telefon",IsApproved = true,IsHome = true},
        new Product() {Name = "Lenovo Laptop", Price = 15000 ,Url = "lenovo-laptop",ImageUrl = "lenovolaptop.jpg",Description = "Samsung Telefon",IsApproved = true,IsHome = true},
        new Product() { Name = "Iphone 16", Price = 40000 , Url = "iphone-16",ImageUrl = "iphone16.jpg",Description = "Iphone telefon",IsApproved = true ,IsHome = true},
        new Product() { Name = "Kulaklik", Price = 1000 , Url = "kulaklik",ImageUrl = "kulaklik.png",Description = "iyi Kulaklik",IsApproved = true,IsHome = false },
        new Product() { Name = "Monster Laptop", Price = 50000 ,Url = "monster-laptop",ImageUrl = "monsterlaptop.png",Description = "iyi laptop",IsApproved = true,IsHome = false }
    };

    private static ProductCategory[] ProductCategories =
    {
        new ProductCategory() { Product = Products[0], Category = Categories[0] },
        new ProductCategory() { Product = Products[1], Category = Categories[0] },
        new ProductCategory() { Product = Products[2], Category = Categories[1] },
        new ProductCategory() { Product = Products[3], Category = Categories[0] },
        new ProductCategory() { Product = Products[4], Category = Categories[2] },
        new ProductCategory() { Product = Products[5], Category = Categories[1] },
    
    };

}
