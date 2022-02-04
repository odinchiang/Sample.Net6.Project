using Microsoft.AspNetCore.Mvc;

namespace Sample.Net6.Project.Controllers
{
    public class Log4NetController : Controller
    {
        /*
            1. Nuget 安裝以下套件
               log4net
               Microsoft.Extensions.Logging.Log4Net.AspNetCore
            2. 配置 config 文件 (ConfigFiles/log4net.Config，直接複製修改，並將此檔案設定為"永遠複製")
            3. Program.cs 加上 log4net 服務
               builder.Logging.AddLog4Net("ConfigFiles/log4net.Config");
            4. 在 Controller 中注入 ILogger 或 ILoggerFactory
            5. 若要將 log 寫入資料庫
               參考：http://logging.apache.org/log4net/release/config-examples.html
               此處以 SqlServer 為例
               * Nuget 安裝 System.Data.SqlClient
               * log4net.Config 加入相關的 appender
               * root 節點中配置 <appender-ref ref="AdoNetAppender_SqlServer" />
         */

        private readonly ILogger<Log4NetController> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public Log4NetController(ILogger<Log4NetController> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;

            _logger.LogInformation($"{GetType().Name} 被建構了...");

            ILogger<Log4NetController> logger2 = _loggerFactory.CreateLogger<Log4NetController>();
            logger2.LogInformation($"{GetType().Name} 被建構了...logger2");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Log4Net/Index 被執行了...");

            ILogger<Log4NetController> logger3 = _loggerFactory.CreateLogger<Log4NetController>();
            logger3.LogInformation("Log4Net/Index 被執行了...logger3");

            return View();
        }
    }
}
