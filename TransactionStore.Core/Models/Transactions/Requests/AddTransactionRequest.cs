using TransactionStore.Core.Enums;
using TransactionStore.Core.DTOs;

namespace TransactionStore.Core.Models.Transactions.Requests;

public class AddTransactionRequest
{
    public AccountDto AccountFrom { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime time { get; set; }
    public AccountDto AccountTo { get; set; }
}