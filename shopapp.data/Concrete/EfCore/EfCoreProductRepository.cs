using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.data.Abstract;
using shopapp.entity;
namespace shopapp.data.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product,ShopContext>, IProductRepository
    {
        public List<Product> GetPopularProducts()
        {
           using (var context = new ShopContext())
           {
               return context.Products.ToList();
           }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(i=> i.IsApproved && i.IsHome).ToList();
            }
        }
        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(i=> i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(name))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category)
                        .Where(p => p.ProductCategories.Any(c => c.Category.Url == name));
                }
                return products.Skip((page-1)*pageSize).Take(pageSize).ToList();
            }
        }

        public List<Product> GetSearchResult(string search)
        {
            using (var context = new ShopContext())
            {
                IQueryable<Product> products = context.Products.Where(i => i.IsApproved);

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(i => i.Name.ToLower().Contains(search.ToLower()) || i.Description.ToLower().Contains(search.ToLower()));
                }

                return products.ToList();
            }
        }


       public int getCountByCategory(string category)
        {
            using (var context = new ShopContext())
            {
                var products = context.Products.Where(i=>i.IsApproved).AsQueryable();
                if (!string.IsNullOrEmpty(category))
                {
                    products = products.Include(i => i.ProductCategories).ThenInclude(i => i.Category)
                        .Where(p => p.ProductCategories.Any(c => c.Category.Url == category));
                }

                return products.Count();
            }
        }
        public Product GetProductDetails(string url)
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(i => i.Url == url).Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category).FirstOrDefault();
            }
        }

        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ShopContext())
            {
                return context.Products.Where(i => i.ProductId == id).Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category).FirstOrDefault();
            }
        }


       public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new ShopContext())
            {
           var product = context.Products.Include(i=>i.ProductCategories).FirstOrDefault(i=>i.ProductId == entity.ProductId);

           if (product != null)
           {
               product.Name = entity.Name;
               product.Description = entity.Description;
               product.Price = entity.Price;
               product.Url = entity.Url;
               product.ImageUrl = entity.ImageUrl;
               product.IsApproved = entity.IsApproved;
               product.IsHome = entity.IsHome;
               
               product.ProductCategories = categoryIds.Select(catid=>new ProductCategory()
               {
                   ProductId = product.ProductId,
                   CategoryId = catid
               }).ToList();
               context.SaveChanges();
           }
            }
        }
    }
}