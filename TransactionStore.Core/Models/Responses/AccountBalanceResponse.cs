using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Responses;

public class AccountBalanceResponse
{
    public Guid AccountId { get; set; }
    public Currency CurrencyType { get; set; }
    public decimal Balance { get; set; }
}