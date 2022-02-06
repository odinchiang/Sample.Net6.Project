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
         * === 授權基本配置 ===
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
         *
         * === 角色授權 ===
         *
         * 1. 標記多個 [Authorize]
         *    給定不同的 Role，當前使用者必須同時包含此處指定的多個 Role 的角色，同時滿足才能授權訪問。
         *    "且"的關係。
         *
         * 2. 標記單個 [Authorize]
         *    以逗號分隔多個 Role，多個角色只要有一個是包含在使用者信息中，就能被授權訪問。
         *    "或"的關係。
         *
         *  === 策略授權 ===
         *
         * 1. 角色授權其實就是一個特殊的策略授權，除了角色，也可以用其他來判斷。
         * 2. 定義策略 (Program.cs)
         * 3. 策略生效：標記策略
         */

        /// <summary>
        /// 在授權的時候，必須要先認證，先找出使用者信息，如果可以找到使用者信息，表示使用者登入過，
        /// 但登入過不表示有權限。
        /// </summary>
        /// <returns></returns>
        //[Authorize] // 預設授權渠道，有使用者信息即授權成功
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Index()
        {
            var user = HttpContext.User;
            return View();
        }

        /// <summary>
        /// 角色授權，Admin 角色才有權限
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 若標記一個角色時，使用者角色(ClaimTypes.Role)有其中一個即可。
        /// </remarks>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        public IActionResult RoleAdmin()
        {
            return View();
        }

        /// <summary>
        /// 角色授權，Admin 或 Teacher 角色均有權限
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 若只標記一個特性，用逗號分隔各種角色時，使用者角色(ClaimTypes.Role)有滿足其中一個即可。
        /// </remarks>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
        public IActionResult RoleAdminTeacher()
        {
            return View();
        }

        /// <summary>
        /// 角色授權，Admin 角色才有權限，登入時 ClaimTypes.Role = "Admin" 及 ""User (必須同時包含這兩種角色)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 若標記多個時，使用者角色(ClaimTypes.Role)必須同時包含列出的所有角色才能授權訪問。
        /// </remarks>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin")]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "User")]
        public IActionResult RoleAdminUser()
        {
            return View();
        }

        /// <summary>
        /// 角色授權，Teacher 角色才有權限，登入時 ClaimTypes.Role = "Teacher"
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Teacher")]
        public IActionResult RoleTeacher()
        {
            return View();
        }

        /// <summary>
        /// 策略授權及 Requirement 擴展
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Policy = "RolePolicy")]
        public IActionResult AuthPolicy()
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
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, "test12345@gmail.com"),
                    new Claim("Name", name),
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
