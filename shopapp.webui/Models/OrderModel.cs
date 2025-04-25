using System.ComponentModel.DataAnnotations;

namespace shopapp.webui.Models;

public class OrderModel
{
    [Required(ErrorMessage = "Ad alanı zorunludur")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Adres alanı zorunludur")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Şehir alanı zorunludur")]
    public string City { get; set; }

    [Required(ErrorMessage = "E-posta alanı zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telefon alanı zorunludur")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Kart sahibi adı zorunludur")]
    public string CardName { get; set; }
    
    public string Note { get; set; }
    
    [Required(ErrorMessage = "Kart numarası zorunludur")]
    [CreditCard(ErrorMessage = "Geçerli bir kart numarası giriniz")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Son kullanma ayı zorunludur")]
    [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Geçerli bir ay giriniz (01-12)")]
    public string ExpirationMonth { get; set; }

    [Required(ErrorMessage = "Son kullanma yılı zorunludur")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Geçerli bir yıl giriniz (4 haneli)")]
    public string ExpirationYear { get; set; }

    [Required(ErrorMessage = "CVC kodu zorunludur")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Geçerli bir CVC kodu giriniz (3 veya 4 haneli)")]
    public string Cvc { get; set; }
    
    public CartModel CartModel { get; set; }
}