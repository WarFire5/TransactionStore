using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Transactions.Responses;

public class AccountBalanceResponse
{
    public Guid AccountId { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Balance { get; set; }
}
