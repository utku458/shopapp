using System;
using System.IO;
using DefaultNamespace;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.entity;
using shopapp.webui.EmailService;
using shopapp.webui.Identity;
using shopapp.webui.Models;

var builder = WebApplication.CreateBuilder(args);

// Yapılandırma dosyalarını yükle
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Servisleri yapılandır
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Middleware'leri yapılandır
ConfigureMiddleware(app);

app.Run();

// **Servisleri Yapılandırma Metodu**
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Identity ve ApplicationContext yapılandırması
    services.AddDbContext<ApplicationContex>(options => options.UseSqlite("Data Source = shopapp.db"));
    services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContex>()
            .AddDefaultTokenProviders();

    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;

        options.SignIn.RequireConfirmedEmail = true;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    });

    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.Cookie = new CookieBuilder
        {
            HttpOnly = true,
            Name = "shopapp.cookie",
            SameSite = SameSiteMode.Strict
        };
    });

    // SQLite için DbContext ekleme
    var connectionString = "Data Source=shopapp.db"; 
    services.AddDbContext<ShopContext>(options => options.UseSqlite(connectionString));

    // Bağımlılık Enjeksiyonu (Dependency Injection)
    services.AddScoped<IProductRepository, EfCoreProductRepository>();
    services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
    services.AddScoped<IProductService, ProductManager>();
    services.AddScoped<ICategoryService, CategoryManager>();
    
    // Email Sender yapılandırması
    services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
    {
        var emailSettings = configuration.GetSection("EmailSender");
        return new SmtpEmailSender(
            emailSettings["Host"],
            int.Parse(emailSettings["Port"]),
            bool.Parse(emailSettings["EnableSSL"]),
            emailSettings["Username"],
            emailSettings["Password"]
        );
    });
    
    services.AddScoped<ICartRepository, EfCoreCartRepository>();
    services.AddScoped<ICartService, CartManager>();
    services.AddScoped<IOrderRepository, EfCoreOrderRepository>();
    services.AddScoped<IOrderService, OrderManager>();

    // Iyzipay ayarlarını yapılandır
    services.Configure<IyzipaySettings>(configuration.GetSection("IyzipaySettings"));

    // TempData için session ekliyoruz
    services.AddSession();

    // MVC ve Controller hizmetlerini ekliyoruz
    services.AddControllersWithViews();

    services.AddAuthorization();
}


// **Middleware Yapılandırma Metodu**
void ConfigureMiddleware(WebApplication app)
{
    var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        SeedDatabase.Seed();
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
        RequestPath = "/modules"
    });

    app.UseSession();

    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        
        endpoints.MapControllerRoute(
            name: "orders",
            pattern: "orders",
            defaults: new { controller = "Cart", action = "GetOrders" }
        );
        
        endpoints.MapControllerRoute(
            name: "cart",
            pattern: "cart",
            defaults: new { controller = "Cart", action = "Index" }
        );
        
        endpoints.MapControllerRoute(
            name: "checkout",
            pattern: "checkout",
            defaults: new { controller = "Cart", action = "Checkout" }
        );

        
        // 1. Search route
        endpoints.MapControllerRoute(
            name: "search",
            pattern: "search",
            defaults: new { controller = "Shop", action = "search" }
        );
        endpoints.MapControllerRoute(
            name: "adminroles",
            pattern: "admin/role/list",
            defaults: new { controller = "Admin", action = "RoleList" }
        );
        
        endpoints.MapControllerRoute(
            name: "adminrolecreate",
            pattern: "admin/role/create",
            defaults: new { controller = "Admin", action = "RoleCreate" }
        );
        
        endpoints.MapControllerRoute(
            name: "adminroleedit",
            pattern: "admin/role/{id?}",
            defaults: new { controller = "Admin", action = "RoleEdit" }
        );

        endpoints.MapControllerRoute(
            name: "adminroledelete",
            pattern: "admin/role/delete",
            defaults: new { controller = "Admin", action = "RoleDelete" }
        );
        
        
        endpoints.MapControllerRoute(
            name: "adminusers",
            pattern: "admin/user/list",
            defaults: new { controller = "Admin", action = "UserList" }
        );
        
             
        endpoints.MapControllerRoute(
            name: "adminuseredit",
            pattern: "admin/user/{id?}",
            defaults: new { controller = "Admin", action = "UserEdit" }
        );

        // Admin işlemleri
        endpoints.MapControllerRoute(
            name: "adminproducts",
            pattern: "admin/products",
            defaults: new { controller = "Admin", action = "ProductList" }
        );
        endpoints.MapControllerRoute(
            name: "adminproductcreate",
            pattern: "admin/products/create",
            defaults: new { controller = "Admin", action = "CreateProduct" }
        );
        endpoints.MapControllerRoute(
            name: "adminproductedit",
            pattern: "admin/products/{id?}",
            defaults: new { controller = "Admin", action = "ProductEdit" }
        );

        // Kategori işlemleri
        endpoints.MapControllerRoute(
            name: "admincategories",
            pattern: "admin/categories",
            defaults: new { controller = "Admin", action = "CategoryList" }
        );
        endpoints.MapControllerRoute(
            name: "admincategorycreate",
            pattern: "admin/categories/create",
            defaults: new { controller = "Admin", action = "CreateCategory" }
        );
        endpoints.MapControllerRoute(
            name: "admincategoryedit",
            pattern: "admin/categories/{id?}",
            defaults: new { controller = "Admin", action = "CategoryEdit" }
        );

        // Ürün listeleme (kategoriye bağlı)
        endpoints.MapControllerRoute(
            name: "products",
            pattern: "products/{category?}",
            defaults: new { controller = "Shop", action = "List" }
        );

        // Ürün detayları
        endpoints.MapControllerRoute(
            name: "productdetails",
            pattern: "{url}",
            defaults: new { controller = "Shop", action = "Details" }
        );

        // Varsayılan route
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );
    });
    SeedIdentity.Seed(userManager, roleManager,configuration).Wait();
}
