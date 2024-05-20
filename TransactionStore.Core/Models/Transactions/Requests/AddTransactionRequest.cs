using TransactionStore.Core.Enums;
using TransactionStore.Core.DTOs;

namespace TransactionStore.Core.Models.Transactions.Requests;

public class AddTransactionRequest
{
    public Guid Id { get; set; }
    public Guid Accountid { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}