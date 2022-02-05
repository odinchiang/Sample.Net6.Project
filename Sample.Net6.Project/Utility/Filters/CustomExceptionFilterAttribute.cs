using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sample.Net6.Project.Models;

namespace Sample.Net6.Project.Utility.Filters
{
    /// <summary>
    /// 當有實現同步及非同步版本時，只會執行非同步的版本
    /// </summary>
    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public CustomExceptionFilterAttribute(IModelMetadataProvider modelMetadataProvider)
        {
            _modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// 同步版本
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnException(ExceptionContext context)
        {
            // 如果沒有處理 Exception，則處理
            if (!context.ExceptionHandled)
            {
                // 判斷是否為 Ajax 請求
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    // 返回 Json
                    context.Result = new JsonResult(new AjaxResultViewModel()
                    {
                        IsSuccess = false,
                        Message = context.Exception.Message,
                        Data = null
                    });
                }
                else
                {
                    // 返回頁面
                    ViewResult result = new ViewResult()
                    {
                        ViewName = "~/Views/Shared/Error.cshtml"
                    };
                    result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                    result.ViewData.Add("Exception", context.Exception);

                    // 斷路器，只要對 Result 賦值，就不繼續往後
                    context.Result = result;
                }

                context.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// 非同步版本
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            // 如果沒有處理 Exception，則處理
            if (!context.ExceptionHandled)
            {
                // 判斷是否為 Ajax 請求
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    // 返回 Json
                    context.Result = new JsonResult(new AjaxResultViewModel()
                    {
                        IsSuccess = false,
                        Message = context.Exception.Message,
                        Data = null
                    });
                }
                else
                {
                    // 返回頁面
                    ViewResult result = new ViewResult()
                    {
                        ViewName = "~/Views/Shared/Error.cshtml"
                    };
                    result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
                    result.ViewData.Add("Exception", context.Exception);

                    // 斷路器，只要對 Result 賦值，就不繼續往後
                    context.Result = result;
                }

                context.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// 判斷是否為 Ajax 請求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
