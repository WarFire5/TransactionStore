using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Responses;

public class TransferResponse
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
