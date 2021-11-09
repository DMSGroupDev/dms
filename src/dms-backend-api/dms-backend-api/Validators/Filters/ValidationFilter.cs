
using dms_backend_api.Model;
using dms_backend_api.Response;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dms_backend_api.Validators.Filters
{
    public partial class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before
            if (!context.ModelState.IsValid && context.ModelState.ErrorCount > 0)
            {
                var errorInModelState = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value?.Errors.Select(x => x.ErrorMessage)).ToList();

                if (!context.HttpContext.Items.TryGetValue("ValidationResult", out var value))
                    return;
                if (value is not ValidationResult vldResult)
                    return;

                var errorResponse = new ErrorResponse
                {
                    Errors = vldResult.Errors.Select(err => new ErrorModel(err)).ToList()
                };

                context.Result = new BadRequestObjectResult(new BasicResponse() { ErrorResponse = errorResponse, Message = "", StatusCode = (int)HttpStatusCode.BadRequest });
                return;
            }
            await next();
            //after 
        }
    }
}

