using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualBasic;
using System.Text;

namespace E_Commerce.API.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInSeconds;

        public RedisCacheAttribute(int durationInSeconds = 60)
        {
            _durationInSeconds = durationInSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            // Get Cache Service From Container

            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var caheKey = CreateCacheKey(context.HttpContext.Request);

            var data = await cacheService.GetDataAsync(caheKey);

            // If Data Exists In Cache ==> Get Data From Cache + Skip EndPoint


            if(!string.IsNullOrEmpty(data))
            {
                context.Result = new ContentResult()
                {
                    Content = data,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;


            }


            // If Data Not Exists ==> Execute EndPoint + Store Result In Cache If Result Is 200OK + Data


            var executedContext = await next.Invoke();
            if(executedContext.Result is OkObjectResult { Value : not null} ok) 
            {
                await cacheService.SetDataAsync(caheKey, ok.Value, TimeSpan.FromSeconds(_durationInSeconds));

            }


           
        }


        private static string CreateCacheKey(HttpRequest request)
        {

            var Key = new StringBuilder();
            Key.Append(request.Path);

            if(request.Query.Any())
            {
                Key.Append('?');
                foreach (var (k, v) in request.Query.OrderBy(x => x.Key))
                {
                    Key.Append(k).Append("=").Append(v).Append('&');
                }
            }

            return Key.ToString();
            
        }
    }
}
