using Grand.Web.Api.DTOs;
using Grand.Web.Api.Services;
using Grand.Web.Api.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace Grand.Web.Api.Controllers;

[Route("api/webhook/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ApiVersion("1.0")]
public class OrdersController(IOrderProcessingService _orderProcessingService,ILogger<OrdersController> _logger,IOrderValidator _orderValidator) : ControllerBase
{
    /// <summary>
    /// Creates a new order and associated customer if not exists
    /// </summary>
    /// <param name="orderDto">Order creation request details</param>
    /// <returns>Created order details</returns>
    /// <response code="201">Order successfully created</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="422">Validation error</response>
    /// <response code="429">Too many requests</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [EnableRateLimiting("api-policy")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody][Required] OrderCreateDto orderDto)
    {
        try
        {
            // Validate request
            var validationResult = await _orderValidator.ValidateAsync(orderDto);
            if (!validationResult.IsValid)
            {
                return UnprocessableEntity(new ValidationProblemDetails(
                    validationResult.ToDictionary()));
            }

            // Process order
            var result = await _orderProcessingService.ProcessOrderAsync(orderDto);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Order creation failed: {Message}", result.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Order Creation Failed",
                    Detail = result.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Return success response
            _logger.LogInformation("Order {OrderNumber} created successfully", result.Data.OrderNumber);
            return Created($"/api/orders/{result.Data.Id}",
                new OrderResponseDto(result.Data));
        }
        catch (Exception ex) when (ex is ValidationException)
        {
            _logger.LogWarning(ex, "Validation error during order creation");
            return BadRequest(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    { "Validation", new[] { ex.Message } }
                }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during order creation");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing the order.",
                    Status = StatusCodes.Status500InternalServerError
                });
        }
    }
}