using TransactionStore.Core.Enums;

namespace TransactionStore.Core.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid Accountid { get; set; }
    public TransactionType TransactionType { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
}