using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace shopapp.webui.Models;

public class CategoryModel
{
   
        public int CategoryId { get; set; }
        
        
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }
        
        
        [Required(ErrorMessage = "Url is required")]
        public string Url { get; set; }

        
    
        public List<Product> Products { get; set; }

  
        
    
}