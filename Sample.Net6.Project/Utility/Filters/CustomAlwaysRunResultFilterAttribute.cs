using Microsoft.AspNetCore.Mvc.Filters;

namespace Sample.Net6.Project.Utility.Filters
{
    public class CustomAlwaysRunResultFilterAttribute : Attribute, IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("CustomAlwaysRunResultFilterAttribute.OnResultExecuting");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("CustomAlwaysRunResultFilterAttribute.OnResultExecuted");
        }
    }
}
