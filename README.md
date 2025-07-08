# eTicaretProject

Bu proje, katmanlı mimari prensiplerine uygun olarak geliştirilen bir e-ticaret uygulamasıdır. ASP.NET Core Web API kullanılmıştır. Projede Entity Framework Core ile SQL Server üzerinde veri yönetimi sağlanmaktadır.

## 🚀 Kullanılan Teknolojiler

- ASP.NET Core Web API
- Entity Framework Core
- Katmanlı Mimari
- SQL Server
- AutoMapper
- Swagger

## 🧱 Katman Yapısı

eTicaretProject
├── Entities -> Veri modelleri (örneğin: Product, Category)
├── DataAccess -> DbContext ve Repository sınıfları
├── Business -> Servis katmanı (iş kuralları)
├── WebAPI -> API Controller'ları

## ⚙️ Kurulum Adımları

1. Repoyu klonlayın:
   ```bash
   git clone https://github.com/talhaondes/eTicaretProject.git

2. SQL Server üzerinde bir veritabanı oluşturun.

3. appsettings.json dosyasına kendi bağlantı cümlenizi girin:
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=eTicaretDB;Trusted_Connection=True;"
}

4.Proje klasöründe aşağıdaki komutları çalıştırın:
 dotnet restore
 dotnet ef database update
 dotnet run

5.Uygulama çalıştığında Swagger arayüzü üzerinden test edebilirsiniz:
 https://localhost:5001/swagger


Özellikler
  Katmanlı mimari yapısı (SOLID prensiplerine uygun)
  Entity Framework ile veritabanı işlemleri
  AutoMapper ile DTO dönüşümleri
  Swagger ile API test imkanı
  Ürün ve kategori CRUD işlemleri
