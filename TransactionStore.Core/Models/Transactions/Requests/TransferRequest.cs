using TransactionStore.Core.Enums;
using TransactionStore.Core.DTOs;

namespace TransactionStore.Core.Models.Transactions.Requests;

public class TransferRequest
{
    public Guid AccountFromId { get; set; }
    public Guid AccountToId { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyFromType { get; set; }
    public CurrencyType CurrencyToType { get; set; }
    public int AmountFrom { get; set; }
    public int AmountTo { get; set; }
    public DateTime Date { get; set; }
}