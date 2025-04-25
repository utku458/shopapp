using System.Collections.Generic;
using shopapp.entity;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;


namespace shopapp.business.Concrete
{
    public class CategoryManager:ICategoryService
    {
        private ICategoryRepository _categoryRepository;

        public CategoryManager(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public Category GetById(int id)
        {
          return _categoryRepository.GetById(id);
        }

        public List<Category> GetAll()
        {
          
            return _categoryRepository.GetAll();
        }

        public void Create(Category entity)
        {
             _categoryRepository.Create(entity);
        }

        public Category GetByIdWithProducts(int categoryId)
        {
          return _categoryRepository.GetByIdWithProducts(categoryId);
        }

        public void Update(Category entity)
        {
            _categoryRepository.Update(entity);
        }

        public void Delete(Category entity)
        {
             _categoryRepository.Delete(entity);
        }

        public string ErrorMessage { get; set; }
        public bool Validation(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}

