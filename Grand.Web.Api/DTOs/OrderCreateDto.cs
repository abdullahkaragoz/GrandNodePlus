using System.ComponentModel.DataAnnotations;

namespace Grand.Web.Api.DTOs;

public class OrderCreateDto
{
    [Required]
    public int OrderNumber { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string StoreId { get; set; }

    [Required]
    public string CustomerId { get; set; }

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    public string OwnerId { get; set; }

    public string SeId { get; set; }

    public bool PickUpInStore { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderItemDto> OrderItems { get; set; } = new();

    public List<OrderTaxDto> OrderTaxes { get; set; } = new();

    public List<string> OrderTags { get; set; } = new();
}

public class OrderItemDto
{
    [Required]
    public string Sku { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

public class OrderTaxDto
{
    [Required]
    public double TaxCode { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal Rate { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }
} 