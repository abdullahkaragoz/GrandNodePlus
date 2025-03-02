using FluentValidation;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Web.Api.DTOs;

namespace Grand.Web.Api.Validators;

public class OrderValidator : AbstractValidator<OrderCreateDto>, IOrderValidator
{
    private readonly IOrderService _orderService;

    public OrderValidator(IOrderService orderService)
    {
        _orderService = orderService;

        RuleFor(x => x.OrderNumber)
            .NotEmpty()
            .MustAsync(async (orderNumber, cancellation) =>
            {
                var existingOrder = await _orderService.GetOrderByNumber(orderNumber);
                return existingOrder == null;
            }).WithMessage("Order number already exists");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.StoreId)
            .NotEmpty();

        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .Must(items => items.Count > 0)
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.OrderItems).ChildRules(item =>
        {
            item.RuleFor(x => x.Sku)
                .NotEmpty()
                .MaximumLength(50);

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0);

            item.RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0);
        });

        RuleForEach(x => x.OrderTaxes).ChildRules(tax =>
        {
            tax.RuleFor(x => x.Rate)
                .InclusiveBetween(0, 100);

            tax.RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0);
        });
    }

    public async Task<ValidationResult> ValidateAsync(OrderCreateDto orderDto)
    {
        var validationResult = await ValidateAsync(orderDto, default);
        return new ValidationResult
        {
            IsValid = validationResult.IsValid,
            Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray())
        };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }

    public Dictionary<string, string[]> ToDictionary()
    {
        return Errors ?? new Dictionary<string, string[]>();
    }
} 