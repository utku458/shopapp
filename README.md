# ShopApp E-Ticaret Projesi

Bu proje, ASP.NET Core MVC kullanılarak geliştirilmiş bir e-ticaret uygulamasıdır.

## Özellikler

- Kullanıcı yönetimi ve kimlik doğrulama
- Ürün ve kategori yönetimi
- Sepet işlemleri
- Sipariş yönetimi
- Ödeme entegrasyonu (Iyzipay)
- E-posta bildirimleri (Mailtrap)

## Kurulum

1. Projeyi klonlayın:
```bash
git clone https://github.com/kullaniciadi/shopapp.git
```

2. Gerekli paketleri yükleyin:
```bash
cd shopapp
dotnet restore
```

3. Veritabanını oluşturun:
```bash
cd shopapp.webui
dotnet ef database update
```

4. Geliştirme ortamı için gerekli ayarları yapın:
- `appsettings.Development.json` dosyasını oluşturun
- Iyzipay ve Mailtrap API anahtarlarınızı ekleyin

5. Uygulamayı çalıştırın:
```bash
dotnet run --environment Development
```

## Geliştirme Ortamı Ayarları

Geliştirme ortamı için `appsettings.Development.json` dosyası oluşturun:

```json
{
  "EmailSender": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": "587",
    "EnableSSL": "true",
    "Username": "your-mailtrap-username",
    "Password": "your-mailtrap-password"
  },
  "IyzipaySettings": {
    "ApiKey": "your-iyzipay-api-key",
    "SecretKey": "your-iyzipay-secret-key",
    "BaseUrl": "https://sandbox-api.iyzipay.com"
  }
}
```

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. # ShopApp E-ticaret Projesi
