using NLog.Web;
using Sample.Net6.Project.Utility.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 加入 log4net 服務
builder.Logging.AddLog4Net("ConfigFiles/log4net.Config");

// 加入 NLog 服務
builder.Logging.AddNLog("ConfigFiles/NLog.config");

// 加入 Session 服務
builder.Services.AddSession();

builder.Services.AddControllersWithViews(mvcOptions =>
{
    // 全域註冊 Filter (對整個專案都生效)
    //mvcOptions.Filters.Add<CustomCacheResourceFilterAttribute>();
});



// ====================================================


var app = builder.Build();

// ====================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// 使用 Session
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
