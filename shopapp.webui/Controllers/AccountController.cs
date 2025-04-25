using System;
using System.Threading.Tasks;
using DefaultNamespace;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using shopapp.webui.EmailService;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers;



public class AccountController:Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private IEmailSender _emailSender;
    private ICartService _cartService;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, ICartService cartService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _cartService = cartService;
    }
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginModel()
        {
            ReturnUrl = returnUrl
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await  _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
            
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError(string.Empty, "Hesabinizi onaylayin.");
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (result.Succeeded)
        {
            return Redirect(!string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : "~/");

        }
        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View(model); 
    }
    
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = model.UserName,
            Email = model.Email
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            
            //generate token 
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = code });
            //email
            Console.Write(url);
           
            await _emailSender.SendEmailAsync("altinayutku0@gmail.com", "Test Subject", $"<a href='https://localhost:5001{url}'>{url}</a>");

            return RedirectToAction("Login", "Account");
        }
        ModelState.AddModelError("","Bilinmeyen bir hata oldu lutfen tekrar deneyin");
        return View(model);
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
        
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            TempData["message"] = "Gecersiz token";
            return View();
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["message"] = "boyle bir kullanici yok ";
            return View();
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            // Cart objesini lustur 
            _cartService.InitalizeCart(user.Id);
            TempData["message"] = "Hesabiniz Onaylandi";
        }
        return View();
    }


    
    
    
    public IActionResult ResetPassword(string userid,string token)
    {
        if (userid == null || token == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var model = new ResetPasswordModel{Token=token};
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction("Login", "Account");
        }
            
        return View(model);
    }
    public IActionResult ForgotPassword()
    {
        return View(); 
    }

    
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email )
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["message"] = "Gecersiz email";
            return View();
        }
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            TempData["message"] = "Gecersiz email";
            return View();
        }
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        
        //generate token 
    
        var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = code });
        //email
        Console.Write(url);
           
        await _emailSender.SendEmailAsync(email, "Test Subject", $"<a href='https://localhost:5001{url}'>{url}</a>");
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}