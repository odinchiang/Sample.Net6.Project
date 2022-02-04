using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sample.Net6.Project.Utility.Filters
{
    public class CustomCacheAsyncResourceFilterAttribute : Attribute, IAsyncResourceFilter
    {
        /// <summary>
        /// 模擬緩存
        /// </summary>
        private static Dictionary<string, object> _cacheDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 當該資源執行時
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            Console.WriteLine("CustomCacheAsyncResourceFilterAttribute.OnResourceExecutionAsync Before");

            string key = context.HttpContext.Request.Path;
            if (_cacheDictionary.ContainsKey(key))
            {
                context.Result = (IActionResult)_cacheDictionary[key];
            }
            else
            {
                // 執行 Controller 的建構子及 Action
                ResourceExecutedContext resource = await next.Invoke();

                _cacheDictionary[key] = resource.Result;

                Console.WriteLine("CustomCacheAsyncResourceFilterAttribute.OnResourceExecutionAsync After");
            }
            
            
        }
    }
}
