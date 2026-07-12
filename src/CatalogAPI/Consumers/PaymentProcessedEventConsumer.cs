using CatalogAPI.Application.Abstractions.Persistence;
using CatalogAPI.Domain.Entities;
using FCG.Contracts.Events;
using MassTransit;

namespace CatalogAPI.Consumers
{
    public class PaymentProcessedEventConsumer
        : IConsumer<PaymentProcessedEvent>
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public PaymentProcessedEventConsumer(
            IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public async Task Consume(
            ConsumeContext<PaymentProcessedEvent> context)
        {
            var paymentProcessedEvent = context.Message;

            if (!string.Equals(
                    paymentProcessedEvent.Status,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var userGameEntity = new UserGameEntity
            {
                UserId = paymentProcessedEvent.UserId,
                GameId = paymentProcessedEvent.GameId
            };

            await _repositoryUoW.UserGameRepository.Add(
                userGameEntity);

            await _repositoryUoW.SaveAsync();
        }
    }
}