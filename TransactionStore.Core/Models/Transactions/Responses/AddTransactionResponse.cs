using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Transactions.Responses;

public class AddTransactionResponse
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}