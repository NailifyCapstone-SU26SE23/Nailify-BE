using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using Nailify.Capstone.Application.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var type = argument.GetType();
                if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(Guid))
                {
                    continue;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(type);
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                        var errorMessage = string.Join("; ", errors);

                        context.Result = new BadRequestObjectResult(new ApiResult<object>(false, errorMessage));
                        return;
                    }
                }
            }

            await next();
        }
    }
}