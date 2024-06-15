using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Requests;

public class TransferRequest
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public Currency CurrencyFromType { get; set; }
    public Currency CurrencyToType { get; set; }
    public decimal Amount { get; set; }
}