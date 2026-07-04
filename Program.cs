using HyliCoffeeWeb.Data;
using HyliCoffeeWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. EF Core + SQL Server =====
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== 2. MVC + Razor Views =====
builder.Services.AddControllersWithViews();

// ===== 3. Session (giỏ hàng lưu trên server, thay localStorage) =====
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ===== 4. Cookie Authentication =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath    = "/Account/Login";
        options.LogoutPath   = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan   = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name  = "HyliCoffee.Auth";
    });

// ===== 5. Authorization Policy =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",    policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer", "Admin"));
});

// ===== 6. Services =====
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

// ===== Auto Migrate + Seed khi khởi động =====
using (var scope = app.Services.CreateScope())
{
    var db     = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    db.Database.Migrate();
    await HyliCoffeeWeb.Data.DbInitializer.SeedAsync(db, hasher);
}

// ===== Middleware =====
// ForwardedHeaders: bắt buộc khi chạy sau IIS/Nginx/Somee
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Chỉ ép HTTPS khi chạy local (dev). Trên Somee, IIS lo việc HTTPS rồi.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

// Bắt lỗi 404/500 → trả về trang Error thân thiện
app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

app.UseRouting();

app.UseSession();       // Session phải đặt SAU UseRouting, TRƯỚC Auth
app.UseAuthentication();
app.UseAuthorization();

// ===== Routes =====

// FIX LỖI 1: Phải dùng MapAreaControllerRoute (không phải MapControllerRoute)
// để ASP.NET Core nhận diện đúng Area "Admin".
// MapControllerRoute với defaults area sẽ KHÔNG route đúng đến Area controllers.
app.MapAreaControllerRoute(
    name:     "admin",
    areaName: "Admin",
    pattern:  "Admin/{controller=Dashboard}/{action=Index}/{id?}");

// Route mặc định cho khách hàng
app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
