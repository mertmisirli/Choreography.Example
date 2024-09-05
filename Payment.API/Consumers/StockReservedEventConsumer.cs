using MassTransit;
using MassTransit.Transports;
using Shared;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint endpoint)
        {
            _publishEndpoint = endpoint;
        }

        public async Task  Consume(ConsumeContext<StockReservedEvent> context)
        {

            if(true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent()
                {
                    OrderId = context.Message.OrderId,
                };


                Console.WriteLine("Ödeme Başarılı ..");
                await _publishEndpoint.Publish(paymentCompletedEvent);

            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new PaymentFailedEvent()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Ödeme Başarısız ...",
                    OrderItems = context.Message.OrderItems
                };

                Console.WriteLine("Ödeme Başarısız ..");
                await _publishEndpoint.Publish(paymentFailedEvent);
            }
        }
    }
}
