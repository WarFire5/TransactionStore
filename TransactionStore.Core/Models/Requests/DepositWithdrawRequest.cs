using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Requests;

public class DepositWithdrawRequest
{
    public Guid AccountId { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
}