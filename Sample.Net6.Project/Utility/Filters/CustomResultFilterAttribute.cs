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
    public class CustomResultFilterAttribute : Attribute, IResultFilter
    {
        /// <summary>
        /// Action 執行後，View 渲染前
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("CustomResultFilterAttribute.OnResultExecuting");

            if (context.Result is JsonResult result)
            {
                context.Result = new JsonResult(new AjaxResultViewModel()
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Data = result.Value
                });
            }
        }

        /// <summary>
        /// View 渲染後
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("CustomResultFilterAttribute.OnResultExecuted");
        }
    }
}
