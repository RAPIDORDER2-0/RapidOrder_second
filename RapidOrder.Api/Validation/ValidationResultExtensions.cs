using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RapidOrder.Api.Validation;

public static class ValidationResultExtensions
{
    public static ModelStateDictionary ToModelStateDictionary(this ValidationResult result)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return modelState;
    }
}
