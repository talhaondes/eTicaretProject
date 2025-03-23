using eTicaretDal.Abstract;
using eTicaretDal.Concrete;
using eTicaretData.Context;
using eTicaretData.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// BA�IMLILIK ENJEKS�YON
builder.Services.AddDbContext<ETicaretContext>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<IOrderDal, OrderDal>();
builder.Services.AddScoped<IProductDal, ProductDal>();
builder.Services.AddScoped<IOrderLineDal, OrderLineDal>();
builder.Services.AddScoped<IAddressDal, AddressDal>();

// Identity kimlik do�rulama 
builder.Services.AddIdentity<AppUser, AppRole>(option =>
{
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 15 dakika s�ren var ��kman i�in
    option.Lockout.MaxFailedAccessAttempts = 5; // 5 kere hatal� giri�
    option.Password.RequireDigit = true; // say� gerektiriyor
    option.Password.RequireUppercase = true; // b�y�k harf gerekyior
    option.Password.RequiredUniqueChars = 1; // �ifrede en az 1 farkl� karakter t�r� gerektiriyor
}).AddEntityFrameworkStores<ETicaretContext>().AddDefaultTokenProviders();

// Kullan�c� verileri ve kimlik bilgilerini y�netmek i�in ETicaretContext �zerinden veri taban�na ba�lan�r.
// �ifre s�f�rlama, e-posta onay� gibi i�lemler i�in gerekli olan token'lar� olu�turacak sa�lay�c�lar� ekler.
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Account/Login"; // giri� sayfas�
    option.LogoutPath = "/Account/Logoutpost"; // ��k�� sayfas�
    option.AccessDeniedPath = "/"; // eri�im reddedildi sayfas�
    option.SlidingExpiration = false; // s�rekli giri�
    option.Cookie = new CookieBuilder
    {
        Name = "AspNetCoreIdentityExampleCookie", // �erez ismi
        HttpOnly = true, // Sadece HTTP �zerinden eri�ilebilsin
        SameSite = SameSiteMode.Lax, // SameSite �zelli�i
        SecurePolicy = CookieSecurePolicy.Always, // HTTPS �zerinden �erez g�nderimi
    };
    option.ExpireTimeSpan = TimeSpan.FromMinutes(15); // �erezin s�resi
});


// Oturum y�netimi
builder.Services.AddSession(); // ASP.NET Core uygulamas�nda session (oturum) y�netimi ekler. Bu komut, oturum verilerini saklamak i�in gereken altyap�y� ve hizmetleri ekler.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP �zerinden yap�lan t�m isteklere otomatik olarak HTTPS y�nlendirmesi yapar. 
app.UseStaticFiles(); // Uygulaman�n statik dosyalar�na (CSS, JavaScript, resimler) eri�im sa�lar.
app.UseSession(); // Oturum y�netimini sa�lar.
app.UseRouting(); // HTTP isteklerini y�nlendirir
app.UseAuthentication(); // Kullan�c� kimli�i do�rulan�r
app.UseAuthorization(); // Kullan�c� yetkilendirilir


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
