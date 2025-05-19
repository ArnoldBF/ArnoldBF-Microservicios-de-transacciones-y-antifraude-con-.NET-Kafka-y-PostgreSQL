using System;


namespace Domain.Entities
{

    public enum TransactionStatus
    {
        Pending,
        Approved,
        Rejected,
    }
    public class Transaction
    {

        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }

        public Guid TargerAccountId { get; set; }

        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}