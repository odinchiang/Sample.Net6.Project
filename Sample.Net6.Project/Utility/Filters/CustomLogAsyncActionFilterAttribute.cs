using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Sample.Net6.Project.Utility.Filters
{
    /// <summary>
    /// 非同步 ActionFilter
    /// </summary>
    public class CustomLogAsyncActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ILogger<CustomLogAsyncActionFilterAttribute> _logger;

        public CustomLogAsyncActionFilterAttribute(ILogger<CustomLogAsyncActionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 執行 Action 前
            var para = context.HttpContext.Request.QueryString.Value;
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，參數為 {para}");

            // 執行 Action
            var excutedContext = await next.Invoke();

            // 執行 Action 後
            var result = JsonConvert.SerializeObject(excutedContext.Result);
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，執行結果為 {result}");
        }
    }
}
