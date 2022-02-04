using Microsoft.AspNetCore.Mvc;

namespace Sample.Net6.Project.Controllers
{
    public class NLogController : Controller
    {
        /*
            1. Nuget 安裝 NLog.Web.AspNetCore
            2. 配置 config 文件 (ConfigFiles/NLog.config，直接複製修改，並將此檔案設定為"永遠複製")
            3. Program.cs 加入服務 builder.Logging.AddNLog("ConfigFiles/NLog.config");
            4. 在 Controller 中注入 ILogger 或 ILoggerFactory (與 log4net 同)
            5. 若要將 log 寫入資料庫
               參考：http://logging.apache.org/log4net/release/config-examples.html
               此處以 SqlServer 為例
               * Nuget 安裝 System.Data.SqlClient
               * NLog.config 加入相關的 target 與 rule
            6. 可用 Log4NetController 測試
         */

        public IActionResult Index()
        {
            return View();
        }
    }
}
