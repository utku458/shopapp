using System.Collections.Generic;
using shopapp.entity;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;


namespace shopapp.business.Concrete
{
    public class ProductManager:IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public Product GetById(int id)
        {
         return _productRepository.GetById(id);
        }

       public List<Product> GetSearchResult(string search)
        {
            return _productRepository.GetSearchResult(search);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public int getCountByCategory(string category)
        {
            return _productRepository.getCountByCategory(category);
        }

        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            return _productRepository.GetProductsByCategory(name,page,pageSize);
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public bool Create(Product entity)
        {
            if (Validation(entity))
            {
                _productRepository.Create(entity);
                return true;
            }
           return  false;
        }
        
        public void Update(Product entity, int[] categoryIds)
        {
            _productRepository.Update(entity, categoryIds);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public string ErrorMessage { get; set; }
        public bool Validation(Product entity)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Name is required \n";
                isValid = false;
            }
            
            if (entity.Price <= 0)
            {
                ErrorMessage += "Price is required \n";
                isValid = false;
            }
            return isValid;
        }
    }

    
}
