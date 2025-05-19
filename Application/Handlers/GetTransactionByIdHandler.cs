using Application.Interfaces;
using Domain.Entities;
using Application.Queries;


namespace Application.Handlers
{
    public class GetTransactionByIdHandler
    {
        private readonly ITransactionRepository _repository;

        public GetTransactionByIdHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Transaction?> Handle(GetTransactionByIdQuery query)
        {
            return await _repository.GetTransactionByIdAsync(query.Id);
        }

    }
}