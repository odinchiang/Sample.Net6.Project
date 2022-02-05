using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
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

#region �t�m�{��

// ��ܨϥέ��ؤ覡�{��
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
{
    // �Y�����ϥΪ̫H���A�{�ҥ��ѡA���v�]���ѡA�h�������w�� Action
    option.LoginPath = "/Auth/Login";

    // ���w�n�J�A���Y�S���v���A�h����ܫ��w�� Action
    option.AccessDeniedPath = "/Home/NoAuthority";
});

#endregion

#region �w�q�������v

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RolePolicy", policyBuilder =>
    {
        policyBuilder.RequireRole("Admin", "User"); // �����ܤֺ����䤤�@�Ө���
        policyBuilder.RequireClaim("Account"); // �����]�t���w�� Claim
        policyBuilder.RequireUserName("Odin"); // ���������S�w�ϥΪ̦W�� (ClaimTypes.Name)

        // �i�P�_��������޿�
        policyBuilder.RequireAssertion(context =>
        {
            return context.User.HasClaim(x => x.Type == ClaimTypes.Role) &&
                   context.User.Claims.First(x => x.Type.Equals(ClaimTypes.Role)).Value == "Admin";
        });
    });
});

#endregion



// ====================================================


var app = builder.Build();

// ====================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

#region ������B�z���`

// �p�G Http �ШD���� Response �����A���O 200�A�N�|�i�J Home/Error ��
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

// �U���O�ۦ���ˤ@�� Response ��X
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

// �ϥ� Session
app.UseSession();

app.UseStaticFiles();

app.UseRouting();

#region �{�һP���v

app.UseAuthentication(); // �{��(Ų�v)
app.UseAuthorization(); // ���v

#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
