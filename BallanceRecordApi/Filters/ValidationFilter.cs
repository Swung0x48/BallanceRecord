using System.Linq;
using System.Threading.Tasks;
using BallanceRecordApi.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BallanceRecordApi.Filters
{
    public class ValidationFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(x => x.ErrorMessage)).ToArray();
                
                var errorResponse = new ErrorResponse();

                foreach (var (key, value) in errorsInModelState)
                {
                    foreach (var subError in value)
                    {
                        var errorModel = new ErrorModel
                        {
                            FieldName = key,
                            Message = subError
                        };
                        
                        errorResponse.Error.Add(errorModel);
                    }
                    
                    context.Result = new BadRequestObjectResult(errorResponse);
                    return;
                }
            }
            
            await next();
        }
    }
}