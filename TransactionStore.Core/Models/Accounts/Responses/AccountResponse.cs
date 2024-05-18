using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Accounts.Responses;

public class AccountResponse
{
    public LeadDto Lead { get; set; }
    public string AccountName { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Balance { get; set; }
    public bool Status { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}