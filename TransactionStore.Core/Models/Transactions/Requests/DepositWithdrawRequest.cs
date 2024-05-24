using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Transactions.Requests;

public class DepositWithdrawRequest
{
    public Guid AccountId { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
}