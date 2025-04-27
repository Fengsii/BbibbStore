using EFENGSI_RAHMANTO_ZALUKHU.Interfaces;
using EFENGSI_RAHMANTO_ZALUKHU.Models;
using EFENGSI_RAHMANTO_ZALUKHU.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

// Replace 'YourDbContext' with the name of your own DbContext derived class.
builder.Services.AddDbContext<ApplicationContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(builder.Configuration.GetConnectionString("MySQLconnection"), serverVersion)
    // The following three options help with debugging, but should
    // be changed or removed for production.
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

builder.Services.AddScoped<IAuthentication, AuthenticationService>();
builder.Services.AddHttpContextAccessor(); // Untuk mengakses HttpContext
builder.Services.AddScoped<ILoginLayout, LoginLayoutService>();
builder.Services.AddScoped<IUserSaldo, UserSaldoService>();
builder.Services.AddScoped<ICategory, CategoryService>();
builder.Services.AddScoped<IProduct, ProductService>();
builder.Services.AddScoped<IProductSize, ProductSizeService>();
builder.Services.AddScoped<IOrder, OrderService>();
builder.Services.AddScoped<IOrderDetail, OrderDetailService>();
builder.Services.AddScoped<IPayment, PaymentService>();
builder.Services.AddScoped<IUserProfile, UserProfileService>();
//builder.Services.AddControllersWithViews();
//memeriksa apakah user sudah login
builder.Services.AddSession(); // Registrasi service




// Konfigurasi Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Konfigurasi session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Konfigurasi cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.SlidingExpiration = true; // Reset waktu kadaluarsa setiap kali pengguna aktif
        options.Cookie.HttpOnly = true; // Cookie tidak bisa diakses melalui JavaScript
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookie hanya dikirim melalui HTTPS
    });

// Kebijakan Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
    options.AddPolicy("RequireAnyRole", policy => policy.RequireRole("Admin", "User"));
});

// Tambahkan layanan authorization
builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//memeriksa apakah user sudah login
app.UseSession(); // Gunakan middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Pastikan ini dipanggil sebelum UseAuthorization
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
pattern: "{controller=Authentication}/{action=Login}/{id?}");
//pattern: "{controller=ProductDetail}/{action=Index}/{id?}");

app.Run();
