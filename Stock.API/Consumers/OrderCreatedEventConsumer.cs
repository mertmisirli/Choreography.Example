using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Models;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDBService;
        ISendEndpointProvider _sendEndpointProvider;
        IPublishEndpoint publishEndpoint;


        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _mongoDBService = mongoDBService;
            _sendEndpointProvider = sendEndpointProvider;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();

           IMongoCollection<Stock.API.Models.Stock> collection =  _mongoDBService.GetCollection<Stock.API.Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
              stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count > orderItem.Count)).AnyAsync());

            }

            if(stockResult.TrueForAll(sr => sr.Equals(true))) 
            {
                // Stock güncellemesi
                foreach (var orderItem in context.Message.OrderItems)
                {
                   Stock.API.Models.Stock stock =   await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
                }
                // Payment uyaracak event fırlatılması

                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                StockReservedEvent stockReservedEvent = new StockReservedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.Id,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {

                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.Id,
                    Message = "Stok Yetersiz"
                };

                await publishEndpoint.Publish(stockNotReservedEvent); 
                // Stok Başarısız 
                // Order uyarılacak 

            }
            throw new NotImplementedException();
        }
    }
}
