using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace shopapp.webui.Models;

public class ProductModel
{
    public int ProductId { get; set; }  
    
    [Required (ErrorMessage ="Please enter the name of the product")]
    public string Name { get; set; }

    
    [Required (ErrorMessage = "Please enter the Url of the product")]
    public string Url { get; set; }
    
    
    [Required (ErrorMessage = "Please enter the Price of the product")]
    public double? Price { get; set; } 
    
    [Required (ErrorMessage = "Please enter the Description of the product")]
    public string Description { get; set; }   
    
    [Required (ErrorMessage = "Please enter the ImageUrl of the product")]
    public string ImageUrl { get; set; }
    public bool IsApproved { get; set; }
    public bool IsHome { get; set; }

   // public List<Category> SelectedCategories { get; set; }

}