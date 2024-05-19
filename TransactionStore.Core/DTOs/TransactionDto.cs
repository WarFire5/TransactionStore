using TransactionStore.Core.Enums;

namespace TransactionStore.Core.DTOs;

public class TransactionDto : IdContainer
{
    public AccountDto Account { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime time { get; set; }
    public bool Status { get; set; }
}