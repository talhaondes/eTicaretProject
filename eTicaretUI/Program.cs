using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretData.Context;
using eTicaretData.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// BAÐIMLILIK ENJEKSÝYON
builder.Services.AddDbContext<ETicaretContext>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<IOrderDal, OrderDal>();
builder.Services.AddScoped<IProductDal, ProductDal>();
builder.Services.AddScoped<IOrderLineDal, OrderLineDal>();
builder.Services.AddScoped<IAddressDal, AddressDal>();

// Identity kimlik doðrulama 
builder.Services.AddIdentity<AppUser, AppRole>(option =>
{
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 15 dakika süren var çýkman için
    option.Lockout.MaxFailedAccessAttempts = 5; // 5 kere hatalý giriþ
    option.Password.RequireDigit = true; // sayý gerektiriyor
    option.Password.RequireUppercase = true; // büyük harf gerekyior
    option.Password.RequiredUniqueChars = 1; // Þifrede en az 1 farklý karakter türü gerektiriyor
}).AddEntityFrameworkStores<ETicaretContext>().AddDefaultTokenProviders();

// Kullanýcý verileri ve kimlik bilgilerini yönetmek için ETicaretContext üzerinden veri tabanýna baðlanýr.
// Þifre sýfýrlama, e-posta onayý gibi iþlemler için gerekli olan token'larý oluþturacak saðlayýcýlarý ekler.
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Account/Login"; // giriþ sayfasý
    option.LogoutPath = "/Account/Logoutpost"; // çýkýþ sayfasý
    option.AccessDeniedPath = "/"; // eriþim reddedildi sayfasý
    option.SlidingExpiration = false; // sürekli giriþ
    option.Cookie = new CookieBuilder
    {
        Name = "AspNetCoreIdentityExampleCookie", // çerez ismi
        HttpOnly = true, // Sadece HTTP üzerinden eriþilebilsin
        SameSite = SameSiteMode.Lax, // SameSite özelliði
        SecurePolicy = CookieSecurePolicy.Always, // HTTPS üzerinden çerez gönderimi
    };
    option.ExpireTimeSpan = TimeSpan.FromMinutes(15); // çerezin süresi
});


// Oturum yönetimi
builder.Services.AddSession(); // ASP.NET Core uygulamasýnda session (oturum) yönetimi ekler. Bu komut, oturum verilerini saklamak için gereken altyapýyý ve hizmetleri ekler.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP üzerinden yapýlan tüm isteklere otomatik olarak HTTPS yönlendirmesi yapar. 
app.UseStaticFiles(); // Uygulamanýn statik dosyalarýna (CSS, JavaScript, resimler) eriþim saðlar.
app.UseSession(); // Oturum yönetimini saðlar.
app.UseRouting(); // HTTP isteklerini yönlendirir
app.UseAuthentication(); // Kullanýcý kimliði doðrulanýr
app.UseAuthorization(); // Kullanýcý yetkilendirilir


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
