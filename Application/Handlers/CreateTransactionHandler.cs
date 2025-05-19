using Application.Interfaces;
using Application.Commands;
using Domain.Entities;
using Domain.Events;

namespace Application.Handlers
{
    public class CreateTransactionHandler
    {

        private readonly ITransactionRepository _transactionRepository;
        private readonly IKafka _kafka;

        public CreateTransactionHandler(ITransactionRepository transactionRepository, IKafka kafka)
        {
            _transactionRepository = transactionRepository;
            _kafka = kafka;

        }

        public async Task<Guid> Handle(CreateTransactionCommand command)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = command.SourceAccountId,
                TargerAccountId = command.TargetAccountId,
                TransferTypeId = command.TransferTypeId,
                Value = command.Value,
            };

            await _transactionRepository.CreateTransactionAsync(transaction);
            var message = new TransactionCreatedMessage
            {
                Id = transaction.Id,
                SourceAccountId = transaction.SourceAccountId,
                TargetAccountId = transaction.TargerAccountId,
                TransferTypeId = transaction.TransferTypeId,
                Value = transaction.Value,
                CreatedAt = transaction.CreatedAt

            };
            await _kafka.Kafka("transactions", message);

            return transaction.Id;

        }

    }
}