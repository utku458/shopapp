using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using shopapp.webui.Identity;
using shopapp.webui.Models;
using shopapp.webui.ViewModels;


namespace shopapp.webui.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController:Controller
{
    
    private IProductService _productService;
    private ICategoryService _categoryService;
    private RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<User> _userManager;

    public AdminController(IProductService productService,
        ICategoryService categoryService,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        
        _productService = productService;
        _categoryService = categoryService;
        _roleManager = roleManager;
        _userManager = userManager;
    }


    public async Task<IActionResult> RoleEdit(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var members = new List<User>();
        var nonMembers = new List<User>();

        foreach (var user in _userManager.Users)
        {
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                members.Add(user);
            }
            else
            {
                nonMembers.Add(user);
            }
        }

        var model = new RoleDetails()
        {
            Role = role,
            Members = members,
            NonMembers = nonMembers
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleEdit([FromForm] RoleEditModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.RoleId))
        {
            TempData["ErrorMessage"] = "Invalid role data.";
            return RedirectToAction("RoleList");
        }

        var role = await _roleManager.FindByIdAsync(model.RoleId);
        if (role == null)
        {
            TempData["ErrorMessage"] = "Role not found.";
            return RedirectToAction("RoleList");
        }

        bool hasErrors = false;
        string errorMessage = "";

        // Kullanıcıları role ekleme
        if (model.IdsToAdd != null)
        {
            foreach (var userId in model.IdsToAdd)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        hasErrors = true;
                        foreach (var error in result.Errors)
                        {
                            errorMessage += error.Description + " ";
                        }
                    }
                }
            }
        }

        // Kullanıcıları rolden çıkarma
        if (model.IdsToDelete != null)
        {
            foreach (var userId in model.IdsToDelete)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                    {
                        hasErrors = true;
                        foreach (var error in result.Errors)
                        {
                            errorMessage += error.Description + " ";
                        }
                    }
                }
            }
        }

        if (hasErrors)
        {
            TempData["ErrorMessage"] = "Error updating role: " + errorMessage;
            return RedirectToAction("RoleEdit", new { id = model.RoleId });
        }

        TempData["SuccessMessage"] = "Role updated successfully!";
        return RedirectToAction("RoleList");
    }



    public IActionResult RoleList()
    {
        return View(_roleManager.Roles);
    }
    
    
    public IActionResult RoleCreate()
    {
        return View();
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleCreate(RoleModel model)
    {
        if (ModelState.IsValid)
        {
            var role = new IdentityRole(model.Name.ToUpper());
            var result = await _roleManager.CreateAsync(role);
            
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Role created successfully!";
                return RedirectToAction("RoleList");
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        
        TempData["ErrorMessage"] = "There was an error creating the role.";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RoleDelete(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        
        if (role == null)
        {
            TempData["ErrorMessage"] = "Role not found.";
            return RedirectToAction("RoleList");
        }

        var result = await _roleManager.DeleteAsync(role);
        
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Role deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "There was an error deleting the role.";
        }
        
        return RedirectToAction("RoleList");
    }
    
    public IActionResult ProductList()
    {
        return View(new ProductListViewModel()
        {
            Products =  _productService.GetAll()
        });
    }
    
    
    public IActionResult CategoryList()
    {
        return View(new CategoryListViewModel()
        {
            Categories =  _categoryService.GetAll()
        });
    }

    [HttpGet]
    public IActionResult CreateProduct()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult CreateProduct(ProductModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = new Product()
            {
                Name = model.Name,
                Url = model.Url,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
            

            };

            if (_productService.Create(entity))
            {
                TempData["message"] = "Product created successfully.";
                return RedirectToAction("ProductList"); 
            }
            else
            {
                TempData["message"] = "Product creation failed.";
                return View();
            }
            
      
        }
        else
        {
            return View(model);
        }
       
          
        
    }
    
    
    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult CreateCategory(CategoryModel model)
    {
        if (ModelState.IsValid)
        {
            var entity = new Category()
            {

                Name = model.Name,
                Url = model.Url,


            };
            _categoryService.Create(entity);
            TempData["message"] = "Categoty created successfully.";
            return RedirectToAction("CategoryList");
        }
        else
        {
            return View(model);
        }

    }

    
    
    [HttpGet]
    public IActionResult ProductEdit(int? id/*int[] categoryIds*/)
    {
        if (id == null)
        {
            return NotFound();
        }
        var entity = _productService.GetByIdWithCategories(id.Value);
   

        if (entity == null)
        {
            return NotFound();
        }

        var model = new ProductModel()
        {
            ProductId = entity.ProductId,
            Name = entity.Name,
            Url = entity.Url,
            Price = entity.Price,
            Description = entity.Description,
            ImageUrl = entity.ImageUrl,
            IsApproved = entity.IsApproved,
            IsHome = entity.IsHome,
          //  SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList()

        };
        
       // ViewBag.Categories = _categoryService.GetAll();
        return View(model);
    }
    
    
    
   [HttpPost]
public async Task<IActionResult> ProductEdit(ProductModel p, IFormFile file)
{
    if (ModelState.IsValid)
    {
        var entity = _productService.GetById(p.ProductId);

        if (entity == null)
        {
            return NotFound();
        }

        entity.Name = p.Name;
        entity.Url = p.Url;
        entity.Price = p.Price;
        entity.Description = p.Description;
        entity.IsApproved = p.IsApproved;
        entity.IsHome = p.IsHome;

        if (file != null && file.Length > 0)
        {
            try
            {
                // Dosya uzantısını al ve geçerli olup olmadığını kontrol et (Opsiyonel)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["error"] = "Geçersiz dosya formatı. Sadece JPG, PNG, GIF, WEBP desteklenir.";
                    return View(p);
                }

                // Rastgele bir dosya adı oluştur
                var randomName = $"{Guid.NewGuid()}{extension}";
                entity.ImageUrl = randomName;

                // Dosya kaydedilecek klasör yolu
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");

                // Klasör yoksa oluştur
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, randomName);

                // Eğer aynı isimde bir dosya varsa sil
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Dosya yüklenirken hata oluştu: {ex.Message}";
                return View(p);
            }
        }

        _productService.Update(entity);
        TempData["message"] = "Ürün başarıyla güncellendi.";
        return RedirectToAction("ProductList");
    }

    ViewBag.Categories = _categoryService.GetAll();
    return View(p);
}

    
    
    [HttpGet]
    public IActionResult CategoryEdit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = _categoryService.GetByIdWithProducts(id.Value);

        if (entity == null)
        {
            return NotFound();
        }

        var model = new CategoryModel()
        {
            CategoryId = entity.CategoryId,
            Name = entity.Name,
            Url = entity.Url,
            Products = entity.ProductCategory?.Select(p => p.Product).ToList() ?? new List<Product>() // 🛠️ Null kontrolü yapıldı!
        };
    
        return View(model);
    }

    
    
    
    [HttpPost]
    public IActionResult CategoryEdit(CategoryModel c)
    {
        
       
        if (ModelState.IsValid)
        {
            var entity = _categoryService.GetById(c.CategoryId);
            if (entity == null)
            {
                return NotFound();
            }

        
            entity.Name = c.Name;
            entity.Url = c.Url;
   
            _categoryService.Update(entity);
            TempData["message"]  = "Category updated successfully.";
            return RedirectToAction("CategoryList");
        }
        else
        {
            return View(c);
        }
  
    }
    
    
    
    

    public IActionResult DeleteProduct(int? productId)
    {
        var entity = _productService.GetById((int)productId);

        if (entity!=null)
        {
            _productService.Delete(entity);
        }
        TempData["message"]  = "Product deleted successfully.";
       return RedirectToAction("ProductList");
    }
    
    public IActionResult DeleteCategory(int? categoryId)
    {
        var entity = _categoryService.GetById((int)categoryId);

        if (entity!=null)
        {
            _categoryService.Delete(entity);
        }
        TempData["message"]  = "Category deleted successfully.";
        return RedirectToAction("CategoryList");
    }

    public IActionResult UserList()
    {
      
        return View(_userManager.Users);
    }
    
    
    public async Task<IActionResult> UserEdit(string id)
    {
      var user =  await _userManager.FindByIdAsync(id);

      if (user != null)
      {
          var selectedRoles = await _userManager.GetRolesAsync(user);
          var roles = _roleManager.Roles.Select(i=>i.Name).ToList();
          ViewBag.Roles = roles;
          return View(new UserDetailModel()
          {
              UserId = user.Id,
              Email = user.Email,
              EmailConfirmed = user.EmailConfirmed,
              FirstName = user.FirstName,
              LastName = user.LastName,
           
              SelectedRoles = selectedRoles
              
          });
      }
      return NotFound();
    }


    [HttpPost]
    public async Task<IActionResult> UserEdit(UserDetailModel model,string[] selectedRoles)
    {
        
        ViewBag.Roles = _roleManager.Roles.Select(i => i.Name).ToList();
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user!=null)
            {
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
             
                
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var userroles = await _userManager.GetRolesAsync(user);
                    selectedRoles = selectedRoles?? new string[] { };
                    await _userManager.AddToRolesAsync(user, selectedRoles.Except(userroles).ToArray());
                    await _userManager.RemoveFromRolesAsync(user, userroles.Except(selectedRoles).ToArray());
                    return RedirectToAction("UserList");
                }

                return View(model);


            }
        }

        return View(model);
    }


}