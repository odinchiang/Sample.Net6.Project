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
    /// 同步 ActionFilter
    /// </summary>
    public class CustomLogActionFilterAttribute : Attribute, IActionFilter
    {
        private readonly ILogger<CustomLogActionFilterAttribute> _logger;

        public CustomLogActionFilterAttribute(ILogger<CustomLogActionFilterAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 在該 Action 執行前
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("CustomActionFilterAttribute.OnActionExecuting");

            var para = context.HttpContext.Request.QueryString.Value;
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，參數為 {para}");
        }

        /// <summary>
        /// 在該 Action 執行之後
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("CustomActionFilterAttribute.OnActionExecuted");

            var result = JsonConvert.SerializeObject(context.Result);
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，執行結果為 {result}");
        }
    }
}
