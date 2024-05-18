using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Transactions.Responses;

public class AddTransactionResponse
{
    public AccountDto AccountFrom { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime time { get; set; }
    public AccountDto AccountTo { get; set; }
}