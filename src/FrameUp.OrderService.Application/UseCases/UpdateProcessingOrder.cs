using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameUp.OrderService.Application.UseCases;

public class UpdateProcessingOrder
{
    private IOrderRepository @object;

    public UpdateProcessingOrder(IOrderRepository @object)
    {
        this.@object = @object;
    }

    public async Task Execute(UpdateProcessingOrderRequest request)
    {
        throw new NotImplementedException();
    }
}
