using Domain.Entities;

namespace Domain.Events
{
    public class TransactionStatusMessage
    {

        public Guid TransactionId { get; set; }
        public TransactionStatus Status { get; set; }

    }
}