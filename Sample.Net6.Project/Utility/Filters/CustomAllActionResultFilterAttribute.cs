using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;

namespace Sample.Net6.Project.Utility.Filters
{
    public class CustomAllActionResultFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger<CustomAllActionResultFilterAttribute> _logger;

        public CustomAllActionResultFilterAttribute(ILogger<CustomAllActionResultFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var para = context.HttpContext.Request.QueryString.Value;
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，參數為 {para}");
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = JsonConvert.SerializeObject(context.Result);
            var controllerName = context.HttpContext.GetRouteValue("controller");
            var actionName = context.HttpContext.GetRouteValue("action");
            _logger.LogInformation($"執行 {controllerName} 控制器的 {actionName} 方法，執行結果為 {result}");
        }

        /// <summary>
        /// 若同時有同步版本及非同步版本存在，則會以非同步版本為主，同步版本不會被執行
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            base.OnResultExecuted(context);
        }

        public override Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            return base.OnResultExecutionAsync(context, next);
        }
    }
}
