using System.ComponentModel.DataAnnotations;

namespace shopapp.webui.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "İsim alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "İsim en fazla 50 karakter olmalıdır.")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Soyisim alanı zorunludur.")]
    [StringLength(50, ErrorMessage = "Soyisim en fazla 50 karakter olmalıdır.")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 30 karakter arasında olmalıdır.")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Şifre tekrar alanı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
    public string RePassword { get; set; }
    
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }
}