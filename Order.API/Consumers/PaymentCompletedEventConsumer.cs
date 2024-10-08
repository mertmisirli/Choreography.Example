﻿using MassTransit;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _context;

        public PaymentCompletedEventConsumer(OrderAPIDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order.API.Models.Order order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.OrderStatu = Enums.OrderStatu.Completed;

                await _context.SaveChangesAsync();
            }
            throw new NullReferenceException();
        }
    }
}
