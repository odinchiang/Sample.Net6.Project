using NLog.Web;
using Sample.Net6.Project.Utility.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// �[�J log4net �A��
builder.Logging.AddLog4Net("ConfigFiles/log4net.Config");

// �[�J NLog �A��
builder.Logging.AddNLog("ConfigFiles/NLog.config");

// �[�J Session �A��
builder.Services.AddSession();

builder.Services.AddControllersWithViews(mvcOptions =>
{
    // ������U Filter (���ӱM�׳��ͮ�)
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

// �ϥ� Session
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
