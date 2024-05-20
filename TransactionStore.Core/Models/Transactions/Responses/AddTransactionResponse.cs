using TransactionStore.Core.DTOs;
using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Models.Transactions.Responses;

public class AddTransactionResponse
{
    public Guid Id { get; set; }
    public Guid Accountid { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}