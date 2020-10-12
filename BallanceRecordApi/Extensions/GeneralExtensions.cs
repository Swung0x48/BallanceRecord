using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BallanceRecordApi.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User is null ? 
                string.Empty : 
                httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}