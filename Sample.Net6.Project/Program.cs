using Microsoft.AspNetCore.Diagnostics;
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

#region 中間件處理異常

// 如果 Http 請求中的 Response 的狀態不是 200，就會進入 Home/Error 中
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

// 下面是自行拼裝一個 Response 輸出
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("<html lang=\"en\"><body>\r\n");
        await context.Response.WriteAsync("ERROR!<br><br>\r\n");
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
        Console.WriteLine($"{exceptionHandlerPathFeature?.Error.Message}");
        Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

        if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
        {
            await context.Response.WriteAsync("File error thrown!<br><br>\r\n");
        }
        await context.Response.WriteAsync("<a href=\"/\">Home</a><br>\r\n");
        await context.Response.WriteAsync("</body></html>\r\n");
        await context.Response.WriteAsync(new string(' ', 512)); // IE padding
    });
});

#endregion

// 使用 Session
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
