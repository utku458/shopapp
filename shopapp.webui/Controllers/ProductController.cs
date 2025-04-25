#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;

using shopapp.entity;

namespace shopapp.webui.Controllers
{
    public class ProductController:Controller
    {


         public IActionResult Details(int id){

          
           
         

       

   
            return View();
        }


            public IActionResult List(int? id, string? q){
/*
            var products = ProductRepository.Products;

              if (id!=null){


                    products = products.Where(p=> p.CategoryId == id).ToList();
                }
              if(!string.IsNullOrEmpty(q)){
                    products = products.Where(i=> i.Name.ToLower().Contains(q.ToLower())).ToList();
              }

           

       

            var productViewModel = new ProductViewModel()
            {
                Products = products
            };
*/
       

   
            return View();
        }



        public IActionResult Index(){

            

   
            return View();
        }
       

    public IActionResult Create()
{
   // ViewBag.Categories = new SelectList(CategoryRepository.Categories,"CategoryId","Name");
    return View(new Product()); // Boş bir model gönderiyoruz
}


        
        
       [HttpPost] 
       public IActionResult Create(Product p)
       {
/*
            if(ModelState.IsValid){

                     ProductRepository.AddProducts(p);
                     return RedirectToAction("List");
            }
             ViewBag.Categories = new SelectList(CategoryRepository.Categories,"CategoryId","Name");*/
            return View();
       
       }



     [HttpGet]
     public IActionResult Edit(int id){

       // ViewBag.Categories = new SelectList(CategoryRepository.Categories,"CategoryId","Name");
        return View();
     }


      [HttpPost]
     public IActionResult Edit(Product p){

       
        //ProductRepository.EditProduct(p);

        return RedirectToAction("list");
     }

     [HttpPost]
     public IActionResult Delete(Product p){

           // ProductRepository.DeleteProduct(p.ProductId);
            return RedirectToAction("list");


     }



    } 
}