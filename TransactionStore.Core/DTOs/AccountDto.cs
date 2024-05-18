using TransactionStore.Core.Enums;

namespace TransactionStore.Core.DTOs;

public class AccountDto : IdContainer
{
    public LeadDto Lead { get; set; }
    public string AccountName { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public bool Status { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}