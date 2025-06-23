using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TestDash.Data;

var builder = WebApplication.CreateBuilder(args);





// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ تفعيل الكوكيز للمصادقة
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/sign_in"; // لو المستخدم مش مسجل دخول
        options.AccessDeniedPath = "/Login/sign_in"; // لو مش عنده صلاحية
    });

builder.Services.AddAuthorization(); // ضروري


builder.Services.AddSession();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles(); // عشان المشروع يقدر يقرأ ملفات js و css

app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=sign_in}/{id?}")
    .WithStaticAssets();


app.Run();
