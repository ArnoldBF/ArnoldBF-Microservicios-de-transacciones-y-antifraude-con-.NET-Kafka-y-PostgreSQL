
using Application.Interfaces;
using Infrastructure.data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;


        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(Guid Id) => await _context.Transactions.FindAsync(Id);

        public async Task<decimal> GetDailyTotalTransactionsAsync(Guid SourceAccountId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Transactions
                .Where(t => t.SourceAccountId == SourceAccountId && t.CreatedAt.Date == today)
                .SumAsync(t => t.Value);
        }

        public async Task UpdateTransactionAsync(Guid Id, TransactionStatus status)
        {
            var transaction = await _context.Transactions.FindAsync(Id);
            if (transaction is null) return;
            if (transaction.Status != TransactionStatus.Pending) return;
            transaction.Status = status;
            await _context.SaveChangesAsync();
        }









    }
}