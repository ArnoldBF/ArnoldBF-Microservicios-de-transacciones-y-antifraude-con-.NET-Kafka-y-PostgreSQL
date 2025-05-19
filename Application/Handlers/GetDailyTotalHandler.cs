using Domain.Entities;
using Application.Interfaces;

using Application.Queries;


namespace Application.Handlers
{
    public class GetDailyTotalHandler
    {

        private readonly ITransactionRepository _repository;

        public GetDailyTotalHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(GetDailyTotalQuery query)
        {
            return await _repository.GetDailyTotalTransactionsAsync(query.SourceAccountId);
        }

    }
}