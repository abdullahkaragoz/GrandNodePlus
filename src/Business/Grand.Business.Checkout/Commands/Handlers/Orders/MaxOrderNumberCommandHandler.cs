﻿using Grand.Business.Core.Commands.Checkout.Orders;
using Grand.Data;
using Grand.Domain.Orders;
using MediatR;

namespace Grand.Business.Checkout.Commands.Handlers.Orders;

public class MaxOrderNumberCommandHandler : IRequestHandler<MaxOrderNumberCommand, int?>
{
    private readonly IRepository<Order> _orderRepository;

    public MaxOrderNumberCommandHandler(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<int?> Handle(MaxOrderNumberCommand request, CancellationToken cancellationToken)
    {
        var count = _orderRepository.Table.Count();
        if (count == 0 && !request.OrderNumber.HasValue)
            return null;

        var max = count > 0 ? _orderRepository.Table.Max(x => x.OrderNumber) : 0;
        if (!request.OrderNumber.HasValue) return max;
        if (request.OrderNumber.Value <= max) return max;
        await _orderRepository.InsertAsync(new Order { OrderNumber = request.OrderNumber.Value, Deleted = true });
        max = request.OrderNumber.Value;
        return max;
    }
}