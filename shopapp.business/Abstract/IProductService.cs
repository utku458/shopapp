using System.Collections.Generic;
using shopapp.entity;
namespace shopapp.business.Abstract
{
    public interface IProductService: IValidator<Product>
    {

		
        Product GetById(int id);
        
        Product GetProductDetails(string url);

		 List<Product> GetHomePageProducts();
 		List<Product>  GetProductsByCategory(string name,int page,int pageSize);
        
        int getCountByCategory(string category);

        List<Product> GetSearchResult(string search);
        
        Product GetByIdWithCategories(int id);
        

        List<Product> GetAll();

        bool Create(Product entity);

        void Update(Product entity);
        
        void Update(Product entity,int[] categoryIds);
        void Delete(Product entity);
    }  
}
