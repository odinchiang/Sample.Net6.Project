using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sample.Net6.Project.Models;

namespace Sample.Net6.Project.Utility.Filters
{
    public class CustomAsyncResultFilterAttribute : Attribute, IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // 渲染結果之前
            if (context.Result is JsonResult result)
            {
                context.Result = new JsonResult(new AjaxResultViewModel()
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Data = result.Value
                });
            }

            // 渲染結果
            ResultExecutedContext executedContext = await next.Invoke();

            // 渲染結果之後

        }
    }
}
