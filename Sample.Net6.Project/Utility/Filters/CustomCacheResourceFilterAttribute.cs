using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sample.Net6.Project.Utility.Filters
{
    public class CustomCacheResourceFilterAttribute : Attribute, IResourceFilter
    {
        /*
         * 1. 定義一個緩存的區域
         * 2. 請求來了，根據緩存的標識判斷，如果有緩存就返回緩存的值
         * 3. 如果沒有緩存，做計算
         * 4. 計算結果保存到緩存中
         */

        /// <summary>
        /// 模擬緩存
        /// </summary>
        private static Dictionary<string, object> _cacheDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 在該資源之前
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string key = context.HttpContext.Request.Path; // 請求的路徑
            if (_cacheDictionary.ContainsKey(key))
            {
                // 若給 Result 賦值，就會中斷繼續執行，直接返回調用方 (不再執行 Action 及 OnResourceExecuted)
                context.Result = (IActionResult)_cacheDictionary[key];
            }

            Console.WriteLine("CustomResourceFilterAttribute.OnResourceExecuting");
        }

        /// <summary>
        /// 在該資源之後
        /// </summary>
        /// <param name="context"></param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string key = context.HttpContext.Request.Path;
            _cacheDictionary[key] = context.Result;

            Console.WriteLine("CustomResourceFilterAttribute.OnResourceExecuted");
        }
    }
}
