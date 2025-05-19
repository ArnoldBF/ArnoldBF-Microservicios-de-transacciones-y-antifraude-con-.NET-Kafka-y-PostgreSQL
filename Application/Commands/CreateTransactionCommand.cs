using System.ComponentModel.DataAnnotations;

namespace Application.Commands
{
    public record CreateTransactionCommand(

        [Required] Guid SourceAccountId,
        [Required] Guid TargetAccountId,
        [Required] int TransferTypeId,
        decimal Value
    );

}