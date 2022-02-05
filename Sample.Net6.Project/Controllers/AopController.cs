using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Sample.Net6.ExceptionService;
using Sample.Net6.Project.Models;
using Sample.Net6.Project.Utility.Filters;

namespace Sample.Net6.Project.Controllers
{
    //[CustomCacheResourceFilter]
    public class AopController : Controller
    {
        /*
         * ASP.NET Core 中的 AOP - Filter
         *
         * 1. AuthorizeAttribute：權限驗證
         *
         * 2. ResourceFilter
         *    適用場景：資源緩存
         * 2-1. IResourceFilter：同步版本資源緩存
         *      範例：Utility/Filters/CustomResourceFilterAttribute.cs
         * 2-2. IAsyncResourceFilter：非同步版本資源緩存
         *      範例：Utility/Filters/CustomCacheAsyncResourceFilterAttribute.cs
         *
         * 3. ActionFilter
         *    適用場景：方法前後的記錄 (記錄日誌)
         *            也可做緩存用，但不適合。
         *    IActionFilter：同步版本 (範例：CustomLogActionFilterAttribute.cs)
         *    IAsyncActionFilter：非同步版本 (範例：CustomLogAsyncActionFilterAttribute.cs)
         *
         *    系統內建的 ActionFilterAttribute 本身是抽象類別，繼承了同步及非同步版本的 IActionFilter 及 IResultFilter
         *    範例：CustomAllActionResultFilterAttribute.cs
         *    若複寫時，同時複寫了同步及非同步版本，則會執行非同步版本。
         *
         * 4. IResultFilter：結果生成前後擴展
         *    適用場景：在渲染視圖和結果的時候，作結果的統一處理。
         *            Json 格式的統一處理 => 若返回的是 Json 資料，通常會對其作統一個格式，常見於 WebAPI。
         *
         * 5. IAlwaysRunResultFilter：響應結果的補充
         *    適用場景：ResourceFilter 中只要對 HttpContext.Result 賦值，就不會再繼續往後執行，
         *            如果在賦值之後還想作其他處理，則可以使用 AlwaysRunResultFilter。
         *
         * 6. IExceptionFilter：異常處理
         *    範例：CustomExceptionFilterAttribute.cs
         *
         * Filter 註冊方式
         * 1) 方法註冊：只對該方法有效。
         * 2) 控制器(類別)註冊：對該 Controller 的所有方法都有效。
         * 3) 全域註冊：在 Program.cs 中註冊 (對整個專案所有方法都有效)
         *    builder.Services.AddControllersWithViews(mvcOptions =>
              {
                mvcOptions.Filters.Add<CustomCacheResourceFilterAttribute>();
              });

         * 匿名支持
         * 如果希望在全域或控制器註冊後，有部分方法想不支援註冊的 Filter 功能
         * 系統提供了 AllowAnonymousAttribute，但有部分可以使用，以下三者不能直接使用，需要擴展支持
         * 1) IResourceFilter
         * 2) IActionFilter
         * 3) IResultFilter
         *
         * ExceptionFilter 無法捕捉到的異常處理
         * 1) 中間件支援 (見 Program.cs)
         * 2) 綜合支援可以捕捉到所有的異常
         * 3) ExceptionFilter + 中間件 = 處理所有異常
         */

        public AopController() {
            Console.WriteLine($"{GetType().FullName} 被建構...");
        }

        #region IResourceFilter

        [CustomCacheResourceFilter]
        public IActionResult Index()
        {
            ViewBag.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.WriteLine("Index Action 被執行...");
            return View();
        }

        [CustomCacheAsyncResourceFilter]
        public IActionResult IAsyncResourceFilter()
        {
            ViewBag.Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.WriteLine("IAsyncResourceFilter Action 被執行...");
            return View();
        }

        #endregion

        #region IActionFilter

        // 因為 CustomLogActionFilter 有注入 log，所以不能直接用 CustomLogActionFilterAttribute
        // 知識點 IOC 容器問題
        //[CustomLogActionFilter()]
        // 改用以下任一種方式均可
        //[TypeFilter(typeof(CustomLogActionFilterAttribute))]
        //[ServiceFilter(typeof(CustomLogActionFilterAttribute))]
        [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
        public IActionResult IActionFilter(int id)
        {
            Console.WriteLine("IActionFilter Action 被執行...");

            ViewBag.User = JsonConvert.SerializeObject(new
            {
                Id = id,
                Name = "Odin - ViewBag",
                Age = 20
            });

            ViewData["UserInfo"] = JsonConvert.SerializeObject(new
            {
                Id = id,
                Name = "Mark - ViewData",
                Age = 30
            });

            object description = "Hello ActionFilter";

            return View(description);
        }

        #endregion

        #region IResultFilter
        
        [CustomResultFilter]
        public IActionResult IResultFilter()
        {
            Console.WriteLine("IResultFilter Action 被執行...");
            return View();
        }

        //[CustomResultFilter]
        [CustomAsyncResultFilter]
        public IActionResult IResultFilterJson()
        {
            Console.WriteLine("IResultFilterJson Action 被執行...");

            // 若回傳 Json，正常做法為傳回一個已定義過的物件
            //return Json(new AjaxResultViewModel()
            //{
            //    // ...
            //});

            // 若回傳 Json，但卻是自行 new 物件，可利用 ResultFilter 對此回傳物件進行格式設定
            return Json(new
            {
                Id = 1,
                Name = "Odin",
                Age = 20
            });
        }

        #endregion

        #region ActionFilter (可同時包含 ActionFilter 及 ResultFilter)

        [TypeFilter(typeof(CustomAllActionResultFilterAttribute))]
        public IActionResult AllActionResultFilter(int id)
        {
            return View();
        }

        #endregion

        #region IAlwaysRunResultFilter

        [CustomCacheResourceFilter]
        [CustomAlwaysRunResultFilter]
        public IActionResult IAlwaysRunResultFilter()
        {
            Console.WriteLine("IAlwaysRunResultFilter Action 被執行...");
            return View();
        }

        #endregion

        #region 擴展 AllowAnonymousAttribute

        // 要測試此特性，請先將 Controller 上的 CustomCacheResourceFilter 的註解取消
        [CustomAllowAnonymous]
        public IActionResult ExpandAllowAnonymousAttribute()
        {
            return Json(new
            {
                Message = "Hello ExpandAllowAnonymousAttribute"
            });
        }

        #endregion

        #region IExceptionFilter

        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        public IActionResult IExceptionFilter()
        {
            throw new Exception("IExceptionFilter 測試");
        }

        #endregion

        #region ExceptionFilter 可以捕捉到的異常

        /*
         * 1. Action 出現沒有處理的異常
         * 2. Action 出現已經處理的異常 (已處理過的異常不會被 ExceptionFilter 捕捉)
         * 3. Service 層的異常
         * 4. View 綁定時出現的異常 (ExceptionFilter 捕捉不到)
         * 5. 不存在的 Url 地址 (404，ExceptionFilter 捕捉不到)
         * 6. 其他 Filter 中發生的異常
         *    ActionFilter 中發生異常 - 可以捕捉到
         *    ResourceFilter 中發生異常 - 捕捉不到
         *    ResultFilter 中發生異常 - 捕捉不到
         * 7. 控制器建構函式出現異常
         */

        /// <summary>
        /// Action 出現沒有處理的異常
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        public IActionResult ActionExceptionNoArrange()
        {
            throw new Exception("Action 出現沒有處理的異常");
        }

        /// <summary>
        /// Action 出現已經處理的異常
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        public IActionResult ActionExceptionArrange()
        {
            try
            {
                throw new Exception("Action 出現已經處理的異常");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        /// <summary>
        /// Service 層的異常
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        public IActionResult ServiceException()
        {
            new ExceptionInfoService().Show();
            return View();
        }

        /// <summary>
        /// View 綁定時出現的異常
        /// </summary>
        /// <returns></returns>
        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        public IActionResult ViewBindingException()
        {
            return View();
        }

        /// <summary>
        ///其他 Filter 中發生的異常
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        [TypeFilter(typeof(CustomExceptionFilterAttribute))]
        //[TypeFilter(typeof(CustomLogActionFilterAttribute))] // ActionFilter 中發生異常 - 可以捕捉到
        //[TypeFilter(typeof(CustomCacheResourceFilterAttribute))] // ResourceFilter 中發生異常 - 捕捉不到
        //[TypeFilter(typeof(CustomResultFilterAttribute))] // ResultFilter 中發生異常 - 捕捉不到
        public IActionResult Index12()
        {
            return View();
        }

        #endregion
    }
}
