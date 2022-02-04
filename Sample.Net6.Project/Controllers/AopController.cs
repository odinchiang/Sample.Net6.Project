using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sample.Net6.Project.Models;
using Sample.Net6.Project.Utility.Filters;

namespace Sample.Net6.Project.Controllers
{
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
         *    IActionFilter：同步版本 (CustomLogActionFilterAttribute.cs)
         *    IAsyncActionFilter：非同步版本 (CustomLogAsyncActionFilterAttribute.cs)
         *
         * 4. IResultFilter：結果生成前後擴展
         *    適用場景：在渲染視圖和結果的時候，作結果的統一處理。
         *            Json 格式的統一處理 => 若返回的是 Json 資料，通常會對其作統一個格式，常見於 WebAPI。
         *
         * 5. IAlwaysRun：響應結果的補充
         *
         * 6. IExceptionFilter：異常處理
         */

        public AopController() {
            Console.WriteLine($"{GetType().FullName} 被建構...");
        }

        #region ResourceFilter

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

        #region ActionFilter

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

        #region ResultFilter
        
        [CustomResultFilter]
        public IActionResult IResultFilter()
        {
            Console.WriteLine("IResultFilter Action 被執行...");
            return View();
        }

        [CustomResultFilter]
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
    }
}
