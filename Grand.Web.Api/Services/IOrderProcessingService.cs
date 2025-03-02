using Grand.Domain.Orders;
using Grand.Web.Api.DTOs;
using Grand.Web.Api.Models;

namespace Grand.Web.Api.Services;

public interface IOrderProcessingService
{
    /// <summary>
    /// Process order creation request
    /// </summary>
    /// <param name="orderDto">Order creation request</param>
    /// <returns>Result containing created order or error message</returns>
    Task<ServiceResult<Order>> ProcessOrderAsync(OrderCreateDto orderDto);
} 