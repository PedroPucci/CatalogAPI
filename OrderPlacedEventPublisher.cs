using FCG.Contracts.Events;
using MassTransit;

namespace CatalogAPI.Messaging
{
    public class OrderPlacedEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderPlacedEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish(
            string userId,
            string email,
            int gameId,
            decimal price)
        {
            var orderPlacedEvent = new OrderPlacedEvent(
                userId,
                email,
                gameId,
                price);

            await _publishEndpoint.Publish(orderPlacedEvent);
        }
    }
}