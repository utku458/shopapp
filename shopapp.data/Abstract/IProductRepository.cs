using System.Collections.Generic;
using shopapp.entity;

namespace shopapp.data.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
      List<Product> GetPopularProducts();
      
      List<Product> GetHomePageProducts();

      Product GetByIdWithCategories(int id);
      List<Product> GetSearchResult(string search);
      
      void Update(Product entity,int[] categoryIds);
      
      List<Product> GetProductsByCategory(string name,int page,int pageSize);
      
      int getCountByCategory(string category);
      
      Product GetProductDetails(string url);
    }
}