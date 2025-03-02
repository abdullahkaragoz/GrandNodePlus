using Grand.Web.Api.DTOs;

namespace Grand.Web.Api.Validators;

public interface IOrderValidator
{
    /// <summary>
    /// Validates order creation request
    /// </summary>
    /// <param name="orderDto">Order creation request to validate</param>
    /// <returns>Validation result</returns>
    Task<ValidationResult> ValidateAsync(OrderCreateDto orderDto);
} 