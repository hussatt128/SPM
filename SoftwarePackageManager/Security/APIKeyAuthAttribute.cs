using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SoftwarePackageManager.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string _apiKeyName= "API_KEY";

        // This counter can be used to allow access for a particular number of calls.
        private int counter = 0;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_apiKeyName, out var userApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            
            var apiKey = configuration.GetValue<string>(_apiKeyName);

            if (apiKey != userApiKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            counter++;

            await next();
            
        }
    }
}
