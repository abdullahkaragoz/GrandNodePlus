using Grand.Domain.Orders;

namespace Grand.Web.Api.DTOs;

public class OrderResponseDto
{
    public string Id { get; set; }
    public int OrderNumber { get; set; }
    public string Code { get; set; }
    public string CustomerId { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderTotal { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Status { get; set; }
    public IEnumerable<OrderItemResponseDto> Items { get; set; }

    public OrderResponseDto(Order order)
    {
        Id = order.Id;
        OrderNumber = order.OrderNumber;
        Code = order.Code;
        CustomerId = order.CustomerId;
        CustomerEmail = order.CustomerEmail;
        CustomerName = $"{order.FirstName} {order.LastName}".Trim();
        OrderTotal = (decimal)order.OrderTotal;
        CreatedOnUtc = order.CreatedOnUtc;
        Status = order.OrderStatusId.ToString();
        Items = order.OrderItems.Select(item => new OrderItemResponseDto
        {
            Sku = item.Sku,
            Quantity = item.Quantity,
        });
    }
}

public class OrderItemResponseDto
{
    public string? Sku { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
} 