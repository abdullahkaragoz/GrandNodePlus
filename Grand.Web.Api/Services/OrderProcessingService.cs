using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Web.Api.DTOs;
using Grand.Web.Api.Models;

namespace Grand.Web.Api.Services;

public class OrderProcessingService(IOrderService _orderService, ICustomerService _customerService, IProductService _productService,
    ILogger<OrderProcessingService> _logger
    ) : IOrderProcessingService
{
    public async Task<ServiceResult<Order>> ProcessOrderAsync(OrderCreateDto orderDto)
    {
        try
        {
            // Validate products
            foreach (var item in orderDto.OrderItems)
            {
                var product = await _productService.GetProductBySku(item.Sku);
                if (product == null)
                {
                    return ServiceResult<Order>.Error($"Product with SKU {item.Sku} not found");
                }
            }

            // Handle customer
            var customer = await _customerService.GetCustomerById(orderDto.CustomerId);
            if (customer == null)
            {
                customer = new Customer
                {
                    Email = orderDto.CustomerEmail,
                    Username = $"{orderDto.FirstName} {orderDto.LastName}".Trim(),
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    CustomerGuid = Guid.NewGuid(),
                    PasswordFormatId = PasswordFormat.Clear,
                    BillingAddress = new Address
                    {
                        Email = orderDto.CustomerEmail,
                        FirstName = orderDto.FirstName,
                        LastName = orderDto.LastName
                    }
                };

                customer.Addresses.Add(new Address
                {
                    Email = orderDto.CustomerEmail,
                    FirstName = orderDto.FirstName,
                    LastName = orderDto.LastName
                });

                await _customerService.InsertCustomer(customer);
                _logger.LogInformation("New customer {CustomerId} created", customer.Id);
            }

            // Create order
            var order = new Order
            {
                OrderGuid = Guid.NewGuid(),
                OrderNumber = orderDto.OrderNumber,
                Code = orderDto.Code,
                StoreId = orderDto.StoreId,
                CustomerId = orderDto.CustomerId,
                CustomerEmail = orderDto.CustomerEmail,
                FirstName = orderDto.FirstName,
                LastName = orderDto.LastName,
                OwnerId = orderDto.OwnerId,
                SeId = orderDto.SeId,
                PickUpInStore = orderDto.PickUpInStore,
                CreatedOnUtc = DateTime.UtcNow,
            };

            // Add taxes
            if (orderDto.OrderTaxes?.Any() == true)
            {
                foreach (var tax in orderDto.OrderTaxes)
                {
                    order.OrderTaxes.Add(new OrderTax
                    {
                        Percent = tax.TaxCode,
                        Amount = (double)tax.Amount
                    });
                }
            }

            // Add tags
            if (orderDto.OrderTags?.Any() == true)
            {
                foreach (var tag in orderDto.OrderTags)
                {
                    order.OrderTags.Add(tag);
                }
            }

            //Complete order
            await _orderService.InsertOrder(order);
            _logger.LogInformation("Order {OrderNumber} created successfully", order.OrderNumber);

            return ServiceResult<Order>.Success(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
            throw;
        }
    }
}