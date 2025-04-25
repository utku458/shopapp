using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace shopapp.webui.Models;

public class LoginModel
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    public string ReturnUrl { get; set; }
    
    
}