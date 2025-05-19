
using Domain.Entities;
namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransactionAsync(Transaction Transaction);
        Task<Transaction?> GetTransactionByIdAsync(Guid Id);

        Task<decimal> GetDailyTotalTransactionsAsync(Guid SourceAccountId);

        Task UpdateTransactionAsync(Guid Id, TransactionStatus status);

    }
}