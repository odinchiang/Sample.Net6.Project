using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Net6.Project.Controllers
{
    public class AuthController : Controller
    {
        /*
         * 授權基本配置
         *
         * 1. 使用中間件 (Program.cs)
         *    app.UseAuthentication(); // 認證(鑑權)
         *    app.UseAuthorization(); // 授權
         *
         * 2. 配置授權過程
         *    
              builder.Services.AddAuthentication(option =>
              {
                  option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                  option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                  option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                  option.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                  option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
              }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
              {
                  option.LoginPath = "/Auth/Login";
              });
         *
         * 3. 授權生效：在 Action 上加上 [Authorize]
         */

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登入頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            if (name == "Odin" && password == "1234")
            {
                // 鑑別使用者相關信息
                var claims = new List<Claim>()
                {
                    new Claim("UserId","1"),
                    new Claim(ClaimTypes.Role,"Admin"),
                    new Claim(ClaimTypes.Role,"User"),
                    new Claim(ClaimTypes.Name,$"{name} - 來自於 Cookies"),
                    new Claim(ClaimTypes.Email,"test12345@gmail.com"),
                    new Claim("Password", password),
                    new Claim("Account", "Administrator"),
                    new Claim("Role", "admin")
                };

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30), // 過期時間：30 分鐘

                }).Wait();

                var user = HttpContext.User;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "帳號或密碼錯誤";
            }

            return await Task.FromResult<IActionResult>(View());
        }
    }
}
